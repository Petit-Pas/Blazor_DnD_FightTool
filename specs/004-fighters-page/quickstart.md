# Quickstart: Fighters Page

## Prerequisites

- .NET 10 SDK (10.0.100) installed.
- Existing solution builds: `dotnet build DnDFightTool.slnx` succeeds before starting.
- Feature 003 (undo/redo buttons) merged — provides the UI affordance for undoing add/remove (already in `CombatStatusComponent`).

## Build & Run

```powershell
# Build solution
dotnet build DnDFightTool.slnx

# Run web host (fastest iteration loop for Blazor pages)
dotnet run --project src/Components/DndUi.Web/DndUi.Web.csproj

# Run all tests
dotnet test DnDFightTool.slnx
```

## Implementation Order

The order below minimizes intermediate broken states and lets each step build on a green tree.

### Step 1 — Domain extensions

1. Add `Guid OriginalCharacterId { get; }` to `FightingCharacter`. Update its constructor to require it. Update `FightingCharacter.Copy(IMapper)` to propagate it.
2. Add `event EventHandler<FightingCharacter>? OnFighterAdded` and `void Restore(FightingCharacter fighter)` to `IFightContext`.
3. Update `FightContext`:
   - `Add(Character)`: pass `character.Id` to the `FightingCharacter` constructor; raise `OnFighterAdded` after insertion.
   - `Remove(FightingCharacter)`: decrement `_monsterCountByOriginalId[fighter.OriginalCharacterId]`; remove key when count hits 0.
   - `Restore(FightingCharacter)`: insert, increment counter for monsters, raise `OnFighterAdded`.
4. Update test factories in `DomainTestsUtilities` to populate `OriginalCharacterId`.
5. Update `FightContextTests` with assertions for the new event and `OriginalCharacterId` propagation.

**Verify**: `dotnet build DnDFightTool.slnx` and `dotnet test tests/Domain/FightTests/FightTests.csproj` both green.

### Step 2 — `SetCurrentFighterCommand.FighterId` → `Guid?`

1. Change `FighterId` to `Guid?` on the command.
2. The handler already supports null (delegates to `ICombatTurnService.SetCurrentTurnFighter(Guid?)`).
3. Update `SetCurrentFighterCommandHandlerTests` to add a "null clears current fighter" case.

**Verify**: Tests pass.

### Step 3 — Initiative query + handler + modal

1. Create `src/Business/DnDQueries/FightQueries/InitiativeRollQuery.cs`.
2. Create `src/UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandler.cs`.
3. Create `InitiativeRollQueryHandlerModal.razor` (+ `.razor.cs`, optional `.razor.css`). Mirror `SaveRollResultQueryHandlerModal` structure: `RollableDialogBase` wrapping a `DiceRollResultInputComponent` for the d20 plus a label showing the dex modifier.

**Verify**: Build green. (No tests — query handlers are not covered in existing project conventions.)

### Step 4 — `AddToFightCommand`

1. Create `src/Business/DnDActions/FightActions/AddToFight/AddToFightCommand.cs`.
2. Create `AddToFightCommandHandler.cs`. Inject `IUndoableMediator` (base), `IFightContext`, `ICharacterRepository`. Implement the algorithm in `data-model.md` (cancel-before-mutation, same-kind inheritance, log sub-command).
3. Create `tests/Business/DnDActionsTests/FightActions/AddToFightCommandHandlerTests.cs` with nested `ExecuteTests`, `UndoTests`, `RedoTests`. Cases:
   - Player added with prompted initiative.
   - Monster (first of kind) prompts and adds.
   - Monster (same kind present) skips prompt, inherits initiative.
   - User cancels prompt → command Canceled, no mutation, no history.
   - Repository miss → Failed.
   - Undo removes the fighter and decrements counter.
   - Redo re-executes (re-prompt or re-inherit depending on current state).

**Verify**: Tests pass.

### Step 5 — `RemoveFromFightCommand`

1. Create `src/Business/DnDActions/FightActions/RemoveFromFight/RemoveFromFightCommand.cs`.
2. Create `RemoveFromFightCommandHandler.cs`. Inject `IFightContext`, `ICombatTurnService`. Implement the algorithm in `data-model.md`.
3. Create `tests/Business/DnDActionsTests/FightActions/RemoveFromFightCommandHandlerTests.cs` with `ExecuteTests`, `UndoTests`, `RedoTests`. Cases:
   - Removes a non-active fighter; counter decremented.
   - Removes the active fighter; sub-command nulls `CurrentTurnFighter`.
   - Undo restores fighter (same instance) and re-increments counter.
   - Undo cascades to `SetCurrentFighter` sub-command when applicable.
   - Fighter id miss → Failed.
   - Redo removes again.

**Verify**: Tests pass.

### Step 6 — Fighters page

1. Create `src/Components/DndUi.Shared/Components/Pages/FightersPage.razor` (+ `.razor.cs`, `.razor.css`).
2. Inject `IUndoableMediator`, `IFightContext`, `ICharacterRepository`, `IDialogService`, `IDialogServiceProvider`.
3. `OnInitializedAsync`: call `DialogServiceProvider.SetDialogService(DialogService)`; subscribe to `OnFighterAdded` and `OnFighterRemoved` (each handler calls `InvokeAsync(StateHasChanged)`).
4. `Dispose`: unsubscribe both events.
5. Render: two columns, grouped Players/Monsters, `+`/`-` buttons dispatch the commands.
6. Sort right list with `OrderByDescending(f => f.InitiativeRoll)`.

**Verify**: `dotnet run --project src/Components/DndUi.Web/DndUi.Web.csproj` and walk through US1–US3 manually.

### Step 7 — Rename Fight page → FightDashboard

1. Rename `FightPage.razor` → `FightDashboardPage.razor` (and the `.razor.cs`).
2. Rename class `FightPage` → `FightDashboardPage`.
3. Change `@page "/fight"` → `@page "/fight-dashboard"`.
4. Verify no other code references `FightPage` by name (tests, layouts).

**Verify**: Build green; navigate to `/fight-dashboard`.

### Step 8 — Update navigation

1. Edit `src/Components/DndUi.Shared/Components/Layout/NavMenu.razor`:
   - Add `<MudNavLink HRef="fighters" Icon="@Icons.Material.Filled.Groups">Fighters</MudNavLink>`.
   - Replace the `Fight` link with `<MudNavLink HRef="fight-dashboard" Icon="@Icons.Material.Filled.AutoFixHigh">FightDashboard</MudNavLink>`.

**Verify**: Both links work; no orphaned `Fight` entry.

### Step 9 — Remove `AddToFight` button from character list editor

1. Edit `CharacterListEditorPage.razor`: delete both `<FightButton OnClick="() => AddToFight(...)" Size=Size.Small />` lines.
2. Edit `CharacterListEditorPage.razor.cs`: delete `AddToFight(Character)` method and the `IFightContext` `[Inject]` if no other reference remains.

**Verify**: Build green; the button no longer appears on the character list editor.

### Step 10 — Full validation

1. `dotnet build DnDFightTool.slnx` — green.
2. `dotnet test DnDFightTool.slnx` — all green.
3. Manual run-through of acceptance scenarios US1–US6 from `spec.md`.

## Files Changed (summary)

### Domain (modified)

| File | Change |
|---|---|
| `src/Domain/Fight/Characters/FightingCharacter.cs` | + `OriginalCharacterId`, ctor signature |
| `src/Domain/Fight/IFightContext.cs` | + `OnFighterAdded`, `Restore` |
| `src/Domain/Fight/FightContext.cs` | counter decrement on `Remove`; `Restore`; raise `OnFighterAdded` |

### Business — Commands (new + modified)

| File | New/Modified |
|---|---|
| `src/Business/DnDActions/FightActions/AddToFight/AddToFightCommand.cs` | NEW |
| `src/Business/DnDActions/FightActions/AddToFight/AddToFightCommandHandler.cs` | NEW |
| `src/Business/DnDActions/FightActions/RemoveFromFight/RemoveFromFightCommand.cs` | NEW |
| `src/Business/DnDActions/FightActions/RemoveFromFight/RemoveFromFightCommandHandler.cs` | NEW |
| `src/Business/DnDActions/TurnActions/SetCurrentFighter/SetCurrentFighterCommand.cs` | MOD — `Guid?` |

### Business — Queries (new)

| File | New/Modified |
|---|---|
| `src/Business/DnDQueries/FightQueries/InitiativeRollQuery.cs` | NEW |

### UI (new + modified)

| File | New/Modified |
|---|---|
| `src/UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandler.cs` | NEW |
| `src/UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandlerModal.razor` | NEW |
| `src/UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandlerModal.razor.cs` | NEW |
| `src/UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandlerModal.razor.css` | NEW (optional) |
| `src/Components/DndUi.Shared/Components/Pages/FightersPage.razor` | NEW |
| `src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.cs` | NEW |
| `src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.css` | NEW |
| `src/Components/DndUi.Shared/Components/Pages/FightPage.razor` → `FightDashboardPage.razor` | RENAMED + route change |
| `src/Components/DndUi.Shared/Components/Pages/FightPage.razor.cs` → `FightDashboardPage.razor.cs` | RENAMED + class rename |
| `src/Components/DndUi.Shared/Components/Layout/NavMenu.razor` | MOD — links |
| `src/Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor` | MOD — remove `FightButton`s |
| `src/Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor.cs` | MOD — remove `AddToFight` |

### Tests (new + modified)

| File | New/Modified |
|---|---|
| `tests/Business/DnDActionsTests/FightActions/AddToFightCommandHandlerTests.cs` | NEW |
| `tests/Business/DnDActionsTests/FightActions/RemoveFromFightCommandHandlerTests.cs` | NEW |
| `tests/Business/DnDActionsTests/TurnActions/SetCurrentFighterCommandHandlerTests.cs` | MOD — null case |
| `tests/Domain/FightTests/FightContextTests.cs` | MOD — `OnFighterAdded`, counter decrement, `Restore`, `OriginalCharacterId` |
| `tests/Domain/DomainTestsUtilities/...` | MOD — factory updates for `OriginalCharacterId` |

## DI Notes

- `UndoableMediator` scans assemblies for handlers automatically; `AddToFightCommandHandler`, `RemoveFromFightCommandHandler`, and `InitiativeRollQueryHandler` will be picked up without explicit registration.
- `IFightContext`, `ICharacterRepository`, `ICombatTurnService`, `IDialogServiceProvider` are already singleton-registered (verified in `MauiProgram.cs` and `DndUi.Web/Program.cs`). No DI changes required.
- `IDialogService` is registered by MudBlazor's `AddMudServices()` (existing).

## Verification Checklist

- [ ] `dotnet build DnDFightTool.slnx` green.
- [ ] `dotnet test DnDFightTool.slnx` green.
- [ ] `/fighters` route loads; players visible on left, none on right when fight empty.
- [ ] Adding a player prompts for initiative; player moves left → right.
- [ ] Adding a second monster of same kind does **not** prompt; same initiative as the first.
- [ ] Adding a monster of a different kind **does** prompt.
- [ ] Cancelling the prompt aborts the add; no entry in undo history (undo button does not light up).
- [ ] `-` button removes a fighter immediately, no confirmation.
- [ ] Undo of add removes fighter; if it was the only of its kind, the next add of that template prompts again.
- [ ] Undo of remove restores fighter (HP, statuses, initiative preserved); if active fighter was removed, the active selection is restored.
- [ ] Right list is ordered by `InitiativeRoll` descending.
- [ ] `/fight-dashboard` shows the previous fight page content.
- [ ] No "Add to fight" button on character list editor.
- [ ] Nav menu: "Fighters" + "FightDashboard"; no "Fight".
