# Data Model: Fighters Page

## Overview

This feature touches three layers:

- **Domain** — minimal additions to `IFightContext` and `FightingCharacter`.
- **Business** — two new commands and one new query.
- **UI** — one new page, one new query handler + modal, removed button, renamed page.

No persistence change. No new dependencies.

---

## Domain Changes

### `FightingCharacter` (modified)

```csharp
public class FightingCharacter : ICharacter
{
    private readonly Character _character;

    // NEW
    public Guid OriginalCharacterId { get; }

    // Constructor now requires the originating template id
    public FightingCharacter(Character character, Guid originalCharacterId)
    {
        _character = character ?? throw new ArgumentNullException(nameof(character));
        OriginalCharacterId = originalCharacterId;
    }

    // ... all existing members unchanged ...
}
```

| Property | Type | Description |
|---|---|---|
| `OriginalCharacterId` | `Guid` | The id of the source `Character` template. For players, equals `_character.Id`. For monsters, equals the **template** id (not the cloned copy's regenerated id). |

**Migration**:
- `FightContext.Add(Character character)` calls `new FightingCharacter(_character, character.Id)` for players and `new FightingCharacter(monsterCopy, character.Id)` for monsters (note: `character.Id`, **not** `monsterCopy.Id`).
- `FightingCharacter.Copy(IMapper)` propagates `OriginalCharacterId` to the clone.
- All call sites that construct `FightingCharacter` directly (tests, factories) get the new parameter.

---

### `IFightContext` (modified)

```csharp
public interface IFightContext
{
    // EXISTING
    void Add(Character character);
    void Remove(FightingCharacter character);
    void Update(FightingCharacter character);
    IEnumerable<FightingCharacter> Fighters { get; }
    event EventHandler<FightingCharacter> OnFighterRemoved;
    event EventHandler<Guid> OnFighterUpdated;
    void NotifyFighterUpdated(Guid fighterId);
    FightingCharacter? this[Guid id] { get; }

    // NEW
    event EventHandler<FightingCharacter>? OnFighterAdded;
    void Restore(FightingCharacter fighter);
}
```

| Member | Description |
|---|---|
| `OnFighterAdded` | Fired by `Add(Character)` and `Restore(FightingCharacter)` after the fighter is in `_fighters`. Payload is the inserted `FightingCharacter`. |
| `Restore(FightingCharacter)` | Re-insert an existing `FightingCharacter` instance (used by `RemoveFromFightCommand.UndoAsync`). Re-increments the per-template counter for monsters. Fires `OnFighterAdded`. **Not** intended for general use — it bypasses the clone-on-add semantics deliberately to faithfully restore a removed instance. |

### `FightContext` (modified — implementation)

| Method | Change |
|---|---|
| `Add(Character)` | Construct `FightingCharacter` with `originalCharacterId = character.Id`. Raise `OnFighterAdded` after insertion. |
| `Remove(FightingCharacter)` | After removing from `_fighters`: if `_monsterCountByOriginalId.ContainsKey(fighter.OriginalCharacterId)`, decrement; remove the key when count reaches 0. Then fire `OnFighterRemoved` (existing). |
| `Restore(FightingCharacter)` | New. Insert into `_fighters[fighter.Id]`. If a monster (heuristic: `fighter.Type == Monster`), increment `_monsterCountByOriginalId[fighter.OriginalCharacterId]` and **do not** rename (the name is preserved on the existing instance). Fire `OnFighterAdded`. |

---

## Business Layer — Commands

### `AddToFightCommand`

```csharp
namespace DnDFightTool.Business.DnDActions.FightActions.AddToFight;

public class AddToFightCommand : CommandBase
{
    public Guid SourceCharacterId { get; }
    public Guid? AddedFighterId { get; set; }       // set on Execute
    public int? InitiativeRoll { get; set; }        // set on Execute (after prompt or inheritance)
    public bool InheritedInitiative { get; set; }   // for redo decision-making

    public AddToFightCommand(Guid sourceCharacterId)
    {
        SourceCharacterId = sourceCharacterId;
    }
}
```

### `AddToFightCommandHandler` (algorithm)

```text
ExecuteAsync(command):
    character = _characterRepository.GetCharacterById(command.SourceCharacterId)
    if character is null:
        return Failed
    
    initiative = null
    inherited = false
    
    if character.Type == Monster:
        sameKind = _fightContext.Fighters.FirstOrDefault(f => f.OriginalCharacterId == character.Id)
        if sameKind is not null:
            initiative = sameKind.InitiativeRoll
            inherited = true
    
    if initiative is null:
        queryResponse = await _mediator.QueryAsync(new InitiativeRollQuery(character.Id))
        if queryResponse.Status == Canceled:
            return Canceled        // no state mutation, no history entry
        initiative = queryResponse.Response
    
    // mutation begins here — past this point, all paths must succeed
    fightersBefore = _fightContext.Fighters.Select(f => f.Id).ToHashSet()
    _fightContext.Add(character)
    addedFighter = _fightContext.Fighters.Single(f => !fightersBefore.Contains(f.Id))
    addedFighter.InitiativeRoll = initiative.Value
    
    command.AddedFighterId = addedFighter.Id
    command.InitiativeRoll = initiative
    command.InheritedInitiative = inherited
    
    await _mediator.SendAsSubCommandAsync(
        new WriteLogCommand($"[b]{addedFighter.Name}[/b] joined the fight (initiative [b]{initiative}[/b])"),
        parentCommand: command)
    
    return Success

UndoAsync(command):
    fighter = _fightContext[command.AddedFighterId.Value]
    if fighter is not null:
        _fightContext.Remove(fighter)   // decrements counter, fires OnFighterRemoved
    await base.UndoAsync(command)        // cascades to WriteLogCommand sub-command

RedoAsync(command):
    // The fight state may have changed (other commands run); the safest redo strategy is
    // to clear sub-commands and re-execute. The same-kind detection on re-execute will
    // either find the still-present same-kind monster (re-inheriting) or prompt again.
    // Per UndoableMediator skill: re-execute redo strategy.
    ClearSubCommands(command)
    await ExecuteAsync(command)
```

> **Note on cancel during redo**: If redo re-executes and the user cancels the new prompt, the redo effectively becomes a no-op. This is acceptable — the user explicitly cancelled.

### `RemoveFromFightCommand`

```csharp
namespace DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;

public class RemoveFromFightCommand : CommandBase
{
    public Guid FighterId { get; }
    public FightingCharacter? RemovedFighter { get; set; }   // captured on Execute for Restore

    public RemoveFromFightCommand(Guid fighterId)
    {
        FighterId = fighterId;
    }
}
```

### `RemoveFromFightCommandHandler` (algorithm)

```text
ExecuteAsync(command):
    fighter = _fightContext[command.FighterId]
    if fighter is null:
        return Failed
    
    command.RemovedFighter = fighter
    
    if _combatTurnService.CurrentTurnFighter?.Id == fighter.Id:
        await _mediator.SendAsSubCommandAsync(
            new SetCurrentFighterCommand(null),
            parentCommand: command)
    
    _fightContext.Remove(fighter)   // fires OnFighterRemoved, decrements counter
    
    await _mediator.SendAsSubCommandAsync(
        new WriteLogCommand($"[b]{fighter.Name}[/b] left the fight"),
        parentCommand: command)
    
    return Success

UndoAsync(command):
    if command.RemovedFighter is not null:
        _fightContext.Restore(command.RemovedFighter)   // fires OnFighterAdded, re-increments counter
    await base.UndoAsync(command)   // cascades — restores SetCurrentFighter sub-command undo, WriteLog undo

RedoAsync(command):
    // Re-execute with the captured fighter (the same instance is still alive in _fighters
    // because Undo restored it; on Redo we need to remove again).
    fighter = _fightContext[command.FighterId]
    if fighter is null:
        // model has shifted — abort redo
        return
    ClearSubCommands(command)
    await ExecuteAsync(command)
```

### `SetCurrentFighterCommand` (modified — existing)

| Member | Before | After |
|---|---|---|
| `FighterId` | `Guid` | `Guid?` |

`SetCurrentFighterCommandHandler` already passes the value straight to `ICombatTurnService.SetCurrentTurnFighter(Guid?)`. Existing call sites pass non-null `Guid` values which implicitly convert to `Guid?` — no behavioral change to existing callers.

---

## Business Layer — Query

### `InitiativeRollQuery`

```csharp
namespace DnDFightTool.Business.DnDQueries.FightQueries;

public class InitiativeRollQuery : QueryBase<int>
{
    public Guid CharacterId { get; }

    public InitiativeRollQuery(Guid characterId)
    {
        CharacterId = characterId;
    }
}
```

| Property | Type | Description |
|---|---|---|
| `CharacterId` | `Guid` | The id of the character whose initiative is being rolled. The handler may use it to display the dexterity modifier on the modal. |

Returns `int` — the raw d20 roll. Cancel returns `Canceled(0)`.

---

## UI Layer — Query Handler

### `InitiativeRollQueryHandler`

```csharp
namespace DnDQueryPrompter.FightQueries;

public class InitiativeRollQueryHandler : QueryHandlerBase<InitiativeRollQuery, int>
{
    private readonly IDialogServiceProvider _dialogServiceProvider;
    
    public InitiativeRollQueryHandler(IDialogServiceProvider dialogServiceProvider)
        => _dialogServiceProvider = dialogServiceProvider;
    
    public async override Task<IQueryResponse<int>> ExecuteAsync(InitiativeRollQuery query)
    {
        var parameters = new DialogParameters<InitiativeRollQueryHandlerModal>
        {
            { x => x.CharacterId, query.CharacterId }
        };
        var dialog = await _dialogServiceProvider.GetDialogService()
            .ShowAsync<InitiativeRollQueryHandlerModal>("Roll for Initiative", parameters);
        var result = await dialog.Result;
        
        if (result is null || result.Canceled)
            return QueryResponse<int>.Canceled(0);
        
        return QueryResponse<int>.Success((int)result.Data!);
    }
}
```

### `InitiativeRollQueryHandlerModal` (parameters)

| Parameter | Type | Description |
|---|---|---|
| `CharacterId` | `Guid` | The character whose initiative is rolled. Used to display dex modifier. |

Internal state:
- `_d20Roll : RawD20RollResult`
- `_dexterityModifier : int` (resolved from `ICharacterRepository` or `IFightContext` based on whether the character is a player or monster template)

On confirm: closes with `DialogResult.Ok(_d20Roll.Result)`.

---

## UI Layer — Pages

### `FightersPage` (new)

| Concern | Detail |
|---|---|
| Route | `@page "/fighters"` |
| Injected | `IUndoableMediator`, `IFightContext`, `ICharacterRepository`, `IDialogService`, `IDialogServiceProvider` |
| Lifecycle | `OnInitializedAsync`: `DialogServiceProvider.SetDialogService(DialogService)`; subscribe to `IFightContext.OnFighterAdded` and `OnFighterRemoved`. `Dispose`: unsubscribe. |
| Layout | Two columns. Left: addable list grouped Players/Monsters. Right: fight roster grouped Players/Monsters, sorted by `InitiativeRoll` desc. |
| Click handlers | `OnAddClick(Character c) => _mediator.SendAsync(new AddToFightCommand(c.Id))`. `OnRemoveClick(FightingCharacter f) => _mediator.SendAsync(new RemoveFromFightCommand(f.Id))`. |
| No auto-navigation | Per FR-017. |

### `FightDashboardPage` (renamed from `FightPage`)

| Change | Detail |
|---|---|
| File rename | `FightPage.razor`/`.razor.cs` → `FightDashboardPage.razor`/`.razor.cs` |
| Class rename | `FightPage` → `FightDashboardPage` |
| Route | `@page "/fight"` → `@page "/fight-dashboard"` |
| Render | Existing render logic already gracefully handles `_selectedFighter == null` (the cascade just delivers `null`; child components guard appropriately). |

### `CharacterListEditorPage` (modified)

| Change | Detail |
|---|---|
| `.razor` | Remove the two `<FightButton OnClick="() => AddToFight(...)" Size=Size.Small />` lines (one in Players panel, one in Monsters panel). |
| `.razor.cs` | Remove the `AddToFight(Character)` method. Remove the `[Inject] IFightContext FightContext` if no longer used after removal (verify before deleting). |

### `NavMenu.razor` (modified)

| Before | After |
|---|---|
| `<MudNavLink HRef="fight">Fight</MudNavLink>` | `<MudNavLink HRef="fighters">Fighters</MudNavLink>`<br>`<MudNavLink HRef="fight-dashboard">FightDashboard</MudNavLink>` |

Icon for Fighters: `Icons.Material.Filled.Groups` (or similar). Icon for FightDashboard: keep `Icons.Material.Filled.AutoFixHigh`.

---

## State Transitions

### Adding a player

```
[user clicks +]
    → AddToFightCommand dispatched
    → repository lookup: Player
    → InitiativeRollQuery → modal → user enters d20
        → cancel: Canceled (no history, no mutation)
        → confirm: continue
    → FightContext.Add(player)              → OnFighterAdded fires
    → fighter.InitiativeRoll = d20
    → WriteLogCommand sub-command
    → Success → command in history
    → page re-renders: player removed from left, added to right
```

### Adding a monster (first of kind)

Same as player above; no inheritance branch taken.

### Adding a monster (same kind already present)

```
[user clicks +]
    → AddToFightCommand dispatched
    → repository lookup: Monster
    → same-kind check: TRUE
    → initiative inherited from sameKind.InitiativeRoll
    → no prompt
    → FightContext.Add(monsterTemplate)     → clones, names "Goblin 2", OnFighterAdded fires
    → fighter.InitiativeRoll = inherited
    → WriteLogCommand
    → Success
```

### Removing the active fighter

```
[user clicks -]
    → RemoveFromFightCommand dispatched
    → fighter is current turn fighter
        → SetCurrentFighterCommand(null) sub-command
            → ICombatTurnService.SetCurrentTurnFighter(null)
            → OnChanged fires
            → FightDashboard's _selectedFighter becomes null on next render
    → FightContext.Remove(fighter)          → counter decremented, OnFighterRemoved fires
    → WriteLogCommand
    → Success
```

### Undo of "remove same-kind monster (not last)"

```
[user clicks undo]
    → RemoveFromFightCommand.UndoAsync
    → FightContext.Restore(removedFighter)  → counter re-incremented, OnFighterAdded fires
    → base.UndoAsync cascades:
        → SetCurrentFighterCommand.UndoAsync (only present if it was the active fighter) — restores previous current id
        → WriteLogCommand.UndoAsync — hides log entry
```

---

## Validation Rules

| Field | Rule | Source |
|---|---|---|
| `AddToFightCommand.SourceCharacterId` | Must resolve to an existing `Character` in `ICharacterRepository`. Otherwise `Failed`. | Handler |
| `RemoveFromFightCommand.FighterId` | Must resolve to a `FightingCharacter` in `IFightContext` at execute time. Otherwise `Failed`. | Handler |
| `InitiativeRollQuery` result | Returned `int` is the raw d20 (no validation; can be 1–20 in practice but no enforced range — matches existing `RawD20RollResult` semantics). | Modal |
| `FightingCharacter.OriginalCharacterId` | Set once via constructor; immutable. | Domain |

No FluentValidation classes are introduced — none of the new types have property-level validation requirements distinct from existing patterns.

---

## Relationships

```
FightersPage
    ├── injects → IUndoableMediator, IFightContext, ICharacterRepository, IDialogService, IDialogServiceProvider
    ├── subscribes to → IFightContext.OnFighterAdded
    ├── subscribes to → IFightContext.OnFighterRemoved
    └── dispatches → AddToFightCommand, RemoveFromFightCommand

AddToFightCommandHandler
    ├── injects → IFightContext, ICharacterRepository
    ├── queries → InitiativeRollQuery (via mediator)
    ├── mutates → FightContext.Add, fighter.InitiativeRoll
    └── sub-commands → WriteLogCommand

RemoveFromFightCommandHandler
    ├── injects → IFightContext, ICombatTurnService
    ├── mutates → FightContext.Remove (and FightContext.Restore on undo)
    └── sub-commands → SetCurrentFighterCommand (when active fighter is removed), WriteLogCommand

InitiativeRollQueryHandler
    └── injects → IDialogServiceProvider
```
