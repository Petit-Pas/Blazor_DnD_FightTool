# Implementation Plan: Turn & Round Tracking

**Branch**: `002-turn-round-track` | **Date**: 2026-04-24 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/002-turn-round-tracking/spec.md`

## Summary

Add structured turn-and-round tracking to the fight screen. A new `ICombatTurnService` (singleton, fight-scoped, in `Domain/Fight/TurnTracking/`) is a state container that holds the ordered turn sequence, round counter, and current turn fighter. A single `StartNextTurnCommand` handles both "Start Combat" and "Next Turn" (same button, different label). On each press, the command: ends the current turn (dispatching `EndTurnCommand`), optionally advances the round (`StartNextRoundCommand` with log entry), sets the current turn fighter, and starts the new turn (`StartTurnCommand` which opens a log block). Undo is fully supported — the command stores `PreviousFighterId` and sub-commands own their own undo data. The service is mutated via setters (`SetCurrentTurnFighter`, `SetCurrentRound`) — no `Advance()`/`Revert()` methods. The "selected fighter" (highlighted in UI, whose actions panel is shown) is pure UI state managed in `FightPage` via a `CascadingValue`; it is no longer part of `IFightContext`. Separately, `ExecuteMartialAttackCommandHandler` is migrated from `OpenBlockCommand`/`CloseBlockCommand` to `OpenScopeCommand`/`CloseScopeCommand` so attacks appear as indented scopes within the active turn block. A new `CombatStatusComponent` replaces the "general fight infos" placeholder in `FightPage.razor`.

## Technical Context

**Language/Version**: C# 14 / .NET 10 (`net10.0`)  
**Primary Dependencies**: UndoableMediator, MudBlazor v8.x  
**Storage**: In-memory (Singleton, session-scoped)  
**Testing**: NUnit 4 + FluentAssertions 7 + FakeItEasy 9  
**Target Platform**: .NET MAUI Hybrid (Windows-first), Blazor  
**Project Type**: Desktop app (MAUI Hybrid)  
**Performance Goals**: N/A — in-memory operations, no hot path  
**Constraints**: No new NuGet dependencies  
**Scale/Scope**: Typical fight: 4–8 fighters, tens of rounds max

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate | Status | Notes |
|---|---|---|
| No new dependencies | PASS | No new NuGet packages. All logic is C# in existing projects. |
| File-scoped namespaces | PASS | All new files use file-scoped namespaces. |
| UndoableMediator for all mutations | PASS | `StartNextTurnCommand` goes through `IUndoableMediator.SendAsync`. |
| Domain has no external deps | PASS | `ICombatTurnService` and `CombatTurnService` are pure domain types with no external dependencies. |
| Layer dependency direction | PASS | Domain ← Business ← UI. Service interface in Domain/Fight; command in DnDActions; component in FightBlazorComponents. |
| No logic in .razor files | PASS | `CombatStatusComponent` has all C# in its `.razor.cs` code-behind. |
| DI via ServiceCollectionExtensions | PASS | Registered in `Fight/IoC/ServiceCollectionExtensions.cs`. |
| Tests for command handlers | PASS | `StartNextTurnCommandHandlerTests`, `EndTurnCommandHandlerTests`, `StartTurnCommandHandlerTests`, `StartNextRoundCommandHandlerTests` with `ExecuteTests`, `UndoTests`, `RedoTests`. |
| Blazor components inherit StylableComponentBase | PASS | `CombatStatusComponent` inherits `StylableComponentBase`. |
| IFightContext is source of truth for fight session | PASS | `ICombatTurnService` is a peer service holding turn state; the selected-fighter concept was moved to pure UI state in `FightPage`. |

**No violations. Gate passed.**

**Post-design constitution re-check**: No additional violations introduced by data model or source tree decisions.

## Project Structure

### Documentation (this feature)

```text
specs/002-turn-round-tracking/
├── plan.md              # This file
├── spec.md              # Feature specification
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (speckit.tasks command — NOT created here)
```

### Source Code Changes

```text
src/
├── Domain/
│   ├── Fight/
│   │   ├── TurnTracking/                              # NEW folder
│   │   │   ├── ICombatTurnService.cs                  # NEW — state container interface
│   │   │   └── CombatTurnService.cs                   # NEW — singleton implementation; no undo stack
│   │   └── IoC/
│   │       └── ServiceCollectionExtensions.cs         # MODIFIED — register ICombatTurnService
│   │
│   └── Logs/
│       ├── IDnDLogService.cs                          # MODIFIED — CloseBlock returns Guid; add ReopenBlock
│       └── DnDLogService.cs                           # MODIFIED — implement CloseBlock→Guid, ReopenBlock
│
├── Business/
│   └── DnDActions/
│       ├── TurnActions/                               # NEW folder
│       │   ├── StartNextTurn/
│       │   │   ├── StartNextTurnCommand.cs            # NEW — stores PreviousFighterId; orchestrator
│       │   │   └── StartNextTurnCommandHandler.cs     # NEW — dispatches EndTurn + StartNextRound + StartTurn
│       │   ├── EndTurn/
│       │   │   ├── EndTurnCommand.cs                  # NEW — stateless; dispatches CloseBlockCommand
│       │   │   └── EndTurnCommandHandler.cs           # NEW
│       │   ├── StartNextRound/
│       │   │   ├── StartNextRoundCommand.cs           # NEW — stores PreviousRound; increments round + logs
│       │   │   └── StartNextRoundCommandHandler.cs    # NEW
│       │   └── StartTurn/
│       │       ├── StartTurnCommand.cs                # NEW — stateless; dispatches OpenBlockCommand
│       │       └── StartTurnCommandHandler.cs         # NEW
│       │
│       ├── MartialAttackActions/
│       │   └── ExecuteMartialAttack/
│       │       └── ExecuteMartialAttackCommandHandler.cs  # MODIFIED — block→scope migration
│       │
│       └── LogActions/
│           ├── OpenBlock/
│           │   └── OpenBlockCommandHandler.cs         # MODIFIED — add UndoAsync: calls CloseBlock()
│           └── CloseBlock/
│               ├── CloseBlockCommand.cs               # MODIFIED — add ClosedBlockId field
│               └── CloseBlockCommandHandler.cs        # MODIFIED — ExecuteAsync stores ClosedBlockId; UndoAsync calls ReopenBlock
│
└── UI/
    └── FightBlazorComponents/
        └── CombatStatus/                              # NEW folder (not under Entities — not entity-specific)
            ├── CombatStatusComponent.razor            # NEW — round/turn display + button
            ├── CombatStatusComponent.razor.cs         # NEW — code-behind
            └── CombatStatusComponent.razor.css        # NEW — scoped styles

src/Components/
└── DndUi.Shared/
    └── Components/
        └── Pages/
            └── FightPage.razor                        # MODIFIED — replace placeholder with CombatStatusComponent

tests/
└── Business/
    └── DnDActionsTests/
        └── TurnActions/                               # NEW folder
            ├── StartNextTurnCommandHandlerTests.cs    # NEW — Execute/Undo/Redo test classes
            ├── EndTurnCommandHandlerTests.cs          # NEW — Execute/Undo test classes
            ├── StartNextRoundCommandHandlerTests.cs   # NEW — Execute/Undo test classes
            └── StartTurnCommandHandlerTests.cs        # NEW — Execute/Undo test classes
```

**Structure decision**: No new projects. `ICombatTurnService` lives in `Domain/Fight/TurnTracking/` (fight-scoped), not in `Domain/Logs` (session-scoped). All turn tracking sits in existing projects, wired through existing IoC methods.

## Complexity Tracking

No constitution violations — this table is intentionally empty.

## Key Design Decisions

### D-001: Single `StartNextTurnCommand` for both "Start Combat" and "Next Turn"

The button label changes after the first press (`service.IsStarted ? "Next Turn" : "Start Combat"`) but the command is the same. On first press, the handler initializes the service and dispatches only `StartTurnCommand`. On subsequent presses, it dispatches `EndTurnCommand`, optionally `StartNextRoundCommand`, then `StartTurnCommand`. Undo works uniformly via command fields + sub-command cascade.

### D-002: Four-command sub-command tree for a turn advance

`StartNextTurnCommand` dispatches up to three sub-commands: `EndTurnCommand`, `StartNextRoundCommand` (conditional), and `StartTurnCommand`. Each leaf command dispatches its own log command (`CloseBlockCommand`, round log, `OpenBlockCommand`). This structure cleanly expresses domain semantics and makes each level a natural extension point:
- Future end-of-turn effects (status expiry, etc.) → add to `EndTurnCommandHandler`
- Future round-change effects → add to `StartNextRoundCommandHandler`
- Future start-of-turn effects (resource refresh, etc.) → add to `StartTurnCommandHandler`
- `StartNextTurnCommand` orchestrates but rarely needs to change

`StartNextTurnCommandHandler.UndoAsync` restores service state via `SetCurrentTurnFighter(PreviousFighterId)`. Sub-commands undo their own state. The UI-selected fighter is managed by `FightPage` listening to `ICombatTurnService.OnChanged`.

### D-003: `IDnDLogService.CloseBlock` signature change: `void` → `Guid`

Returns the closed block's `Guid` so `CloseBlockCommand` can store it as `ClosedBlockId` for use in `UndoAsync → ReopenBlock`. `OpenBlock` remains `void`. No existing callers of `OpenBlock` need to change.

### D-004: `ExecuteMartialAttackCommandHandler` migrated from block to scope

The attack handler's `OpenAttackLog` / `CloseAttackLog` methods change from `OpenBlockCommand`/`CloseBlockCommand` to `OpenScopeCommand`/`CloseScopeCommand`. No other handler currently uses block commands, so the migration scope is exactly one file.

### D-005: Service is a state container; `StartNextTurnCommand` owns the orchestration

`ICombatTurnService` exposes state (`CurrentTurnFighter`, `CurrentRound`, `TurnOrder`), queries (`GetNextFighter()`, `IsLastTurnOfRound()`), and setters (`SetCurrentTurnFighter`, `SetCurrentRound`). It does not have `Advance()`/`Revert()` methods — commands drive all transitions and own their undo data.

On the first press, `StartNextTurnCommandHandler.ExecuteAsync` detects `!service.IsStarted` and calls `service.Initialize(fightContext.Fighters)` to sort fighters before computing the first turn.
