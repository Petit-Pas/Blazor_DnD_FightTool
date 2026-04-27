# Data Model: Turn & Round Tracking

**Feature**: 002-turn-round-tracking  
**Date**: 2026-04-24

---

## New Domain Types

### `ICombatTurnService` (interface)

**Location**: `src/Domain/Fight/TurnTracking/ICombatTurnService.cs`  
**Namespace**: `DnDFightTool.Domain.Fight.TurnTracking`  
**Lifetime**: Singleton

| Member | Signature | Description |
|---|---|---|
| `IsStarted` | `bool { get; }` | `true` when `CurrentTurnFighter` is not null. Drives button label. |
| `CurrentRound` | `int { get; }` | 1-based round counter. 0 until started. |
| `CurrentTurnFighter` | `FightingCharacter? { get; }` | The fighter whose turn it currently is. `null` until started. Derived from internal index into `TurnOrder`. |
| `TurnOrder` | `IReadOnlyList<FightingCharacter> { get; }` | Ordered list of fighters (descending initiative, tie-break by insertion order). Computed once during `Initialize`. |
| `Initialize` | `void Initialize(IEnumerable<FightingCharacter> fighters)` | Sorts fighters by `InitiativeSortKey` into `TurnOrder`. Resets state. Called on first turn. |
| `GetNextFighter` | `FightingCharacter GetNextFighter()` | Returns the next fighter in `TurnOrder` after the current one, wrapping to index 0 when at the end. If not started, returns the first fighter. Does **not** mutate state. |
| `IsLastTurnOfRound` | `bool IsLastTurnOfRound()` | Returns `true` if the current fighter is the last in `TurnOrder`. Used by `StartNextTurnCommand` to decide whether to dispatch `StartNextRoundCommand`. |
| `SetCurrentTurnFighter` | `void SetCurrentTurnFighter(Guid? fighterId)` | Sets the current turn fighter by ID lookup in `TurnOrder`. Pass `null` to clear (undo first turn). Fires `OnChanged`. |
| `SetCurrentRound` | `void SetCurrentRound(int round)` | Sets the round counter. Fires `OnChanged`. |
| `OnChanged` | `event Action?` | Fired after `SetCurrentTurnFighter`, `SetCurrentRound`, and `Initialize`. |

**Invariants**:
- `TurnOrder` is fixed after `Initialize` — no mutation mid-fight.
- The service is a pure state container — commands drive all transitions.

---

### `CombatTurnService` (implementation)

**Location**: `src/Domain/Fight/TurnTracking/CombatTurnService.cs`  
**Namespace**: `DnDFightTool.Domain.Fight.TurnTracking`

| Internal field | Type | Description |
|---|---|---|
| `_turnOrder` | `List<FightingCharacter>` | Sorted fighters. Set by `Initialize`. |
| `_currentIndex` | `int` | Index of `CurrentTurnFighter` in `_turnOrder`. -1 when not started. |
| `_currentRound` | `int` | Current round number. 0 until started. |

`SetCurrentTurnFighter(Guid? fighterId)` logic:
1. If `fighterId` is `null` → `_currentIndex = -1`.
2. Else → `_currentIndex = _turnOrder.FindIndex(f => f.Id == fighterId)`. Throw if not found.
3. Fire `OnChanged`.

`SetCurrentRound(int round)` logic:
1. `_currentRound = round`.
2. Fire `OnChanged`.

`GetNextFighter()` logic:
1. If `_currentIndex == -1` → return `_turnOrder[0]` (first fighter).
2. Else → return `_turnOrder[(_currentIndex + 1) % _turnOrder.Count]`.

`IsLastTurnOfRound()` logic:
1. Return `_currentIndex == _turnOrder.Count - 1`.

---

## Modified Domain Types

### `IDnDLogService` — 2 additions

**Location**: `src/Domain/Logs/IDnDLogService.cs` (existing)

| Change | Details |
|---|---|
| `CloseBlock` return type | `void` → `Guid`. Returns the closed block's `Id` so `CloseBlockCommand` can store it as `ClosedBlockId`. |
| New method `ReopenBlock` | `void ReopenBlock(Guid blockId)` — sets `_currentBlock` to the block with the given `Id`. Throws `InvalidOperationException` if the block is not found. |

**`DnDLogService` changes** (`src/Domain/Logs/DnDLogService.cs`):
- `CloseBlock` returns `_currentBlock.Id` before clearing `_currentBlock`.
- `ReopenBlock(Guid blockId)` sets `_currentBlock = _blocks.First(b => b.Id == blockId)` (throws if not found).

**`CloseBlockCommand` additions** (`src/Business/DnDActions/LogActions/CloseBlock/CloseBlockCommand.cs`):
- Add `ClosedBlockId` property (`Guid`). Set in `CloseBlockCommandHandler.ExecuteAsync` from the return value of `CloseBlock()`.

**`CloseBlockCommandHandler`** (`src/Business/DnDActions/LogActions/CloseBlock/CloseBlockCommandHandler.cs`):
- `ExecuteAsync`: `command.ClosedBlockId = _logService.CloseBlock()`.
- `UndoAsync`: `_logService.ReopenBlock(command.ClosedBlockId)`.

**`OpenBlockCommandHandler`** (`src/Business/DnDActions/LogActions/OpenBlock/OpenBlockCommandHandler.cs`):
- `UndoAsync`: `_ = _logService.CloseBlock()` — reverses the block opening.

No callers of `OpenBlock` need to be updated.

---

## New Command Types

### `StartNextTurnCommand`

**Location**: `src/Business/DnDActions/TurnActions/StartNextTurn/StartNextTurnCommand.cs`

| Field | Type | Description |
|---|---|---|
| `PreviousFighterId` | `Guid?` | ID of the fighter whose turn just ended. Null on first turn. Stored for undo. |

`StartNextTurnCommandHandler.ExecuteAsync`:
1. `command.PreviousFighterId = service.CurrentTurnFighter?.Id`
2. If `service.IsStarted` → `SendAsSubCommandAsync(new EndTurnCommand())`
3. If `!service.IsStarted` → `service.Initialize(fightContext.Fighters)`
4. `var nextFighter = service.GetNextFighter()`
5. If `service.IsLastTurnOfRound() || !service.IsStarted` → `SendAsSubCommandAsync(new StartNextRoundCommand())`
6. `service.SetCurrentTurnFighter(nextFighter.Id)`
7. `SendAsSubCommandAsync(new StartTurnCommand())`

`StartNextTurnCommandHandler.UndoAsync`:
1. (Sub-commands undo in reverse: `StartTurnCommand`, `StartNextRoundCommand` if dispatched, `EndTurnCommand`)
2. `service.SetCurrentTurnFighter(command.PreviousFighterId)`

`StartNextTurnCommandHandler.RedoAsync`:
1. `ClearSubCommands(command)`
2. `await ExecuteAsync(command)`

---

### `EndTurnCommand`

**Location**: `src/Business/DnDActions/TurnActions/EndTurn/EndTurnCommand.cs`

No state fields. Dispatches `CloseBlockCommand` as a sub-command. Encapsulates all end-of-turn effects (currently just block closure; future effects such as status expiry slot here).

**Undo**: `EndTurnCommandHandler` has **no explicit `UndoAsync`**. The sub-command cascade undoes `CloseBlockCommand`, whose handler calls `_logService.ReopenBlock(ClosedBlockId)` automatically.

---

### `StartNextRoundCommand`

**Location**: `src/Business/DnDActions/TurnActions/StartNextRound/StartNextRoundCommand.cs`

| Field | Type | Description |
|---|---|---|
| `PreviousRound` | `int` | Round number before the increment. Stored for undo. |

`StartNextRoundCommandHandler.ExecuteAsync`:
1. `command.PreviousRound = service.CurrentRound`
2. `service.SetCurrentRound(command.PreviousRound + 1)`
3. Dispatch a log sub-command for the round header (e.g., "Round {N}")

`StartNextRoundCommandHandler.UndoAsync`:
1. (Log sub-command undoes first via cascade)
2. `service.SetCurrentRound(command.PreviousRound)`

---

### `StartTurnCommand`

**Location**: `src/Business/DnDActions/TurnActions/StartTurn/StartTurnCommand.cs`

No state fields. Dispatches `OpenBlockCommand($"{fighter}'s turn")` as a sub-command. The fighter name is read from `service.CurrentTurnFighter` at execution time. Encapsulates all start-of-turn effects (currently just block opening; future effects slot here).

**Undo**: `StartTurnCommandHandler` has **no explicit `UndoAsync`**. The sub-command cascade undoes `OpenBlockCommand`, whose handler calls `_logService.CloseBlock()` automatically.

---

## UI State

The **selected fighter** (whose actions panel is shown) is **pure UI state** managed by `FightPage`:
- `FightPage.razor.cs` holds a `_selectedFighter` field.
- On turn change (`ICombatTurnService.OnChanged`), `_selectedFighter` is set to `CurrentTurnFighter`.
- On tile click, `_selectedFighter` is set via an `EventCallback<FightingCharacter>`.
- `_selectedFighter` is passed down as a `CascadingValue` named `"SelectedFighter"`.
- `FightingCharacterTile` and `MartialAttackSelectorComponent` receive it as `[CascadingParameter]`.

The `CombatStatusComponent` (Blazor) computes display strings from `ICombatTurnService`:
- **Round display**: `$"Round {service.CurrentRound}"` (shows nothing or "—" when `!service.IsStarted`)
- **Turn display**: `$"{service.CurrentTurnFighter?.Name}'s turn"` (shows nothing when not started)
- **Button label**: `service.IsStarted ? "Next Turn" : "Start Combat"`
- **Button disabled**: `!fightContext.Fighters.Any()`

No new domain entities are required for the UI layer.

---

## State Transition Diagram

```
[No fight loaded]
       │ fighter(s) added to IFightContext
       ▼
[Fight ready, not started]
  IsStarted = false, CurrentRound = 0
  Button: "Start Combat" (enabled if ≥1 fighter)
       │ StartNextTurnCommand → Initialize, StartNextRound(0→1), SetCurrentTurnFighter, StartTurn
       ▼
[Round 1, Fighter A's turn]
  IsStarted = true, CurrentRound = 1
  Log: "Fighter A's turn" block open
  Button: "Next Turn"
       │ StartNextTurnCommand → EndTurn, SetCurrentTurnFighter, StartTurn
       ▼
[Round 1, Fighter B's turn]
  Log: "Fighter A's turn" block closed; "Fighter B's turn" block open
       │ StartNextTurnCommand → EndTurn, StartNextRound(1→2), SetCurrentTurnFighter, StartTurn
       ▼
[Round 2, Fighter A's turn]
  CurrentRound = 2
       │ Undo → sub-commands reverse; SetCurrentTurnFighter(previousId)
       ▼
[Round 1, Fighter B's turn]  ← state restored by command undo
```
