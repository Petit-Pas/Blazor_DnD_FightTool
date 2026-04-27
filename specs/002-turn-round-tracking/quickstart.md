# Quickstart: Turn & Round Tracking

**Feature**: 002-turn-round-tracking  
**Date**: 2026-04-24  
**Purpose**: Developer reference for working with the turn/round tracking system.

---

## How the System Works

Combat flows through a single command: `StartNextTurnCommand`.

1. Before the first press, the fight is in an "unstarted" state. `ICombatTurnService.IsStarted == false`.
2. The first `StartNextTurnCommand` initializes the turn order, starts Round 1, and begins the first fighter's turn.
3. Each subsequent command ends the current turn, advances to the next fighter (dispatching `StartNextRoundCommand` when the last fighter's turn ends), and starts the new turn.
4. Undoing a `StartNextTurnCommand` restores the previous fighter and round number via the turn service state and sub-command cascade. The command stores `PreviousFighterId` for undo; `StartNextRoundCommand` stores `PreviousRound`.

The turn order is computed once from `IFightContext.Fighters` sorted by `FightingCharacter.InitiativeSortKey` (descending initiative, tie-break by insertion order). It never changes mid-fight.

`ICombatTurnService` is a pure state container — it holds turn order, current fighter, round number, and fires events. Commands drive all transitions.

**Active fighter (UI-only state)**: The "selected fighter" whose actions panel is shown is **not** part of the domain. It is managed by `FightPage.razor.cs` as a local `_selectedFighter` field, cascaded to child components via `CascadingValue`. When a turn starts, `FightPage` auto-selects the current turn fighter; when the user clicks a tile, `FightPage` updates the selection.

---

## How to Send `StartNextTurnCommand`

From the `CombatStatusComponent` button click:

```csharp
// In CombatStatusComponent.razor.cs
await _mediator.SendAsync(new StartNextTurnCommand());
```

No arguments — the handler reads state from the service and stores undo data on the command.

---

## How `StartNextTurnCommand` Works Internally

```text
StartNextTurnCommandHandler.ExecuteAsync:
  1. command.PreviousFighterId = service.CurrentTurnFighter?.Id
  2. if service.IsStarted:
       SendAsSubCommandAsync(new EndTurnCommand())
         └─ EndTurnCommandHandler.ExecuteAsync:
              SendAsSubCommandAsync(new CloseBlockCommand())
                └─ CloseBlockCommandHandler: stores ClosedBlockId; UndoAsync → ReopenBlock(ClosedBlockId)
              (EndTurnCommandHandler has NO explicit UndoAsync — the sub-command cascade handles it)
  3. if !service.IsStarted:
       service.Initialize(fightContext.Fighters)
  4. nextFighter = service.GetNextFighter()
  5. if service.IsLastTurnOfRound() || !service.IsStarted:
       SendAsSubCommandAsync(new StartNextRoundCommand())
         └─ StartNextRoundCommandHandler.ExecuteAsync:
              command.PreviousRound = service.CurrentRound
              service.SetCurrentRound(PreviousRound + 1)
              dispatch log sub-command for round header
  6. service.SetCurrentTurnFighter(nextFighter.Id)
  7. SendAsSubCommandAsync(new StartTurnCommand())
       └─ StartTurnCommandHandler.ExecuteAsync:
            SendAsSubCommandAsync(new OpenBlockCommand("{fighter}'s turn"))
              └─ cascade undo → OpenBlockCommandHandler.UndoAsync → _logService.CloseBlock()
                 (StartTurnCommandHandler has NO explicit UndoAsync — the sub-command cascade handles it)

StartNextTurnCommandHandler.UndoAsync:
  (sub-commands undo in reverse: StartTurn, StartNextRound if present, EndTurn)
  service.SetCurrentTurnFighter(command.PreviousFighterId)  // null → clears

StartNextTurnCommandHandler.RedoAsync:
  ClearSubCommands(command)
  await ExecuteAsync(command)   ← standard redo pattern
```

Note: The selected/active fighter in the UI is **not** managed by the command — `FightPage` subscribes to `ICombatTurnService.OnChanged` and updates its local `_selectedFighter` field.

---

## How to Write Log Entries Inside a Turn

Any command handler that produces log output during a fighter's turn should use **scopes**, not blocks. The turn block is already open when the handler runs.

```csharp
// ✅ Correct — open a scope, write entries, close the scope
await _mediator.SendAsSubCommandAsync(new OpenScopeCommand(), command);
await _mediator.SendAsSubCommandAsync(new WriteLogCommand("Attack roll: [b]17[/b]"), command);
await _mediator.SendAsSubCommandAsync(new CloseScopeCommand(), command);

// ❌ Wrong — do not open a new block inside a turn
await _mediator.SendAsSubCommandAsync(new OpenBlockCommand("My Action"), command);
```

---

## How `ExecuteMartialAttackCommandHandler` Changed

Before this feature, each attack opened its own log **block**. After this feature, each attack opens a log **scope** inside the active turn block.

| | Before | After |
|---|---|---|
| `OpenAttackLog` | `SendAsSubCommandAsync(new OpenBlockCommand(...))` | `SendAsSubCommandAsync(new OpenScopeCommand())` |
| `CloseAttackLog` | `SendAsSubCommandAsync(new CloseBlockCommand())` | `SendAsSubCommandAsync(new CloseScopeCommand())` |

Undo behaviour is identical — sub-command undo hides the scope's entries, making the scope invisible.

---

## ICombatTurnService Quick Reference

```csharp
// Check if combat has started
bool started = _combatTurnService.IsStarted;

// Current round (0 if not started)
int round = _combatTurnService.CurrentRound;

// Current active fighter (null if not started)
FightingCharacter? fighter = _combatTurnService.CurrentTurnFighter;

// Get the next fighter in initiative order (does NOT mutate state)
FightingCharacter next = _combatTurnService.GetNextFighter();

// Check if current fighter is last in the round
bool isLast = _combatTurnService.IsLastTurnOfRound();

// The full ordered list of fighters (set at Initialize time)
IReadOnlyList<FightingCharacter> order = _combatTurnService.TurnOrder;

// Subscribe to changes (call StateHasChanged in Blazor component)
_combatTurnService.OnChanged += StateHasChanged;
```

---

## IDnDLogService — New Members

```csharp
// CloseBlock now returns the closed block's Guid (was void)
// Used internally by CloseBlockCommandHandler.ExecuteAsync:
Guid closedBlockId = _logService.CloseBlock();

// Re-open a previously closed block
// Used internally by CloseBlockCommandHandler.UndoAsync:
_logService.ReopenBlock(closedBlockId);
```

`OpenBlock(string name)` remains `void` — no existing callers need to change.

---

## FightPage.razor Change

Replace the placeholder div in `FightPage.razor`:

```razor
{{!-- Before --}}
<div style="grid-row:2; grid-column:2;">
    general fight infos
</div>

{{!-- After --}}
<CombatStatusComponent style="grid-row:2; grid-column:2;" />
```

---

## DI Registration

`ICombatTurnService` / `CombatTurnService` are registered as Singleton in `src/Domain/Fight/IoC/ServiceCollectionExtensions.cs` alongside `IFightContext`.

No new projects or NuGet packages are introduced.
