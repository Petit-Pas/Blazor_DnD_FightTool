# Tasks: Turn & Round Tracking

**Input**: Design documents from `/specs/002-turn-round-tracking/`
**Prerequisites**: plan.md ✅ · spec.md ✅ · research.md ✅ · data-model.md ✅ · quickstart.md ✅

## Format: `[ID] [P?] [Story?] Description with file path`

- **[P]**: Can run in parallel (different files, no incomplete dependencies)
- **[Story]**: User story label (US1–US4)
- No story label: Setup / Foundational / Polish phases

---

## Phase 1: Setup

**Purpose**: Wire the new service into the DI container. Everything else depends on this being present.

- [X] T001 Register `ICombatTurnService` / `CombatTurnService` as Singleton in `src/Domain/Fight/IoC/ServiceCollectionExtensions.cs`

**Checkpoint**: DI wiring in place — all handler constructors can now inject `ICombatTurnService`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: New domain types and modified existing types that every user story depends on. No user story work can begin until this phase is complete.

**⚠️ CRITICAL**: US1–US4 all depend on the foundational types introduced here.

- [X] T002 [P] Create `ICombatTurnService` interface in `src/Domain/Fight/TurnTracking/ICombatTurnService.cs` — `IsStarted`, `CurrentRound`, `CurrentTurnFighter`, `TurnOrder`, `Initialize`, `GetNextFighter`, `IsLastTurnOfRound`, `SetCurrentTurnFighter`, `SetCurrentRound`, `OnChanged`
- [X] T003 Implement `CombatTurnService` in `src/Domain/Fight/TurnTracking/CombatTurnService.cs` — `_turnOrder`, `_currentIndex`, `_currentRound`; implement all interface members per data-model logic
- [X] T005 [P] Modify `src/Domain/Logs/IDnDLogService.cs` — change `CloseBlock()` return type `void` → `Guid`; add `void ReopenBlock(Guid blockId)`
- [X] T006 [P] Implement `IDnDLogService` changes in `src/Domain/Logs/DnDLogService.cs` — `CloseBlock()` returns `_currentBlock.Id` before clearing; `ReopenBlock(blockId)` sets `_currentBlock = _blocks.First(b => b.Id == blockId)`
- [X] T007 Add `ClosedBlockId` property (`Guid`) to `src/Business/DnDActions/LogActions/CloseBlock/CloseBlockCommand.cs`
- [X] T008 Update `src/Business/DnDActions/LogActions/CloseBlock/CloseBlockCommandHandler.cs` — `ExecuteAsync`: `command.ClosedBlockId = _logService.CloseBlock()`; add `UndoAsync`: `_logService.ReopenBlock(command.ClosedBlockId)`
- [X] T009 [P] Add `UndoAsync` to `src/Business/DnDActions/LogActions/OpenBlock/OpenBlockCommandHandler.cs` — body: `_ = _logService.CloseBlock()`

> T003 depends on T002. T007 depends on T005/T006. T008 depends on T007. T009 depends on T005/T006. T005/T006 are independent.

**Checkpoint**: Domain types and modified log infrastructure are ready. All US1–US4 tasks can now begin.

---

## Phase 3: User Story 1 — Advance to the Next Fighter's Turn (Priority: P1) 🎯 MVP

**Goal**: The game master can press "Start Combat" / "Next Turn" to advance through turns and rounds. Undo/redo works correctly. Current turn fighter is tracked in `ICombatTurnService`. Round counter increments when the last fighter's turn ends.

**Independent Test**: Load a fight with 2+ fighters. Press "Start Combat" → verify Round 1, Fighter A active. Press "Next Turn" → verify Fighter B active. Press "Next Turn" again (last fighter) → verify Round 2, Fighter A active. Undo twice → back to Round 1, Fighter A.

### Sub-command implementations (all parallelizable)

- [X] T010 [P] [US1] Create `src/Business/DnDActions/TurnActions/EndTurn/EndTurnCommand.cs` — `CommandBase`, no fields
- [X] T011 [P] [US1] Create `src/Business/DnDActions/TurnActions/EndTurn/EndTurnCommandHandler.cs` — `ExecuteAsync`: `SendAsSubCommandAsync(new CloseBlockCommand())`; no `UndoAsync` override (sub-command cascade handles it)
- [X] T012 [P] [US1] Create `src/Business/DnDActions/TurnActions/StartNextRound/StartNextRoundCommand.cs` — `CommandBase`, property `PreviousRound` (`int`)
- [X] T013 [P] [US1] Create `src/Business/DnDActions/TurnActions/StartNextRound/StartNextRoundCommandHandler.cs` — `ExecuteAsync`: store `PreviousRound`, `SetCurrentRound(+1)`, dispatch `WriteLogCommand($"Round {service.CurrentRound}")` sub-command; `UndoAsync`: `SetCurrentRound(PreviousRound)` (log sub-command undoes automatically)
- [X] T014 [P] [US1] Create `src/Business/DnDActions/TurnActions/StartTurn/StartTurnCommand.cs` — `CommandBase`, no fields
- [X] T015 [P] [US1] Create `src/Business/DnDActions/TurnActions/StartTurn/StartTurnCommandHandler.cs` — `ExecuteAsync`: `SendAsSubCommandAsync(new OpenBlockCommand($"{service.CurrentTurnFighter!.Name}'s turn"))`; no `UndoAsync` override

### Orchestrator (depends on T010–T015)

- [X] T016 [US1] Create `src/Business/DnDActions/TurnActions/StartNextTurn/StartNextTurnCommand.cs` — `CommandBase`, property `PreviousFighterId` (`Guid?`)
- [X] T017 [US1] Create `src/Business/DnDActions/TurnActions/StartNextTurn/StartNextTurnCommandHandler.cs` — full `ExecuteAsync` per data-model step 1–7; `UndoAsync` per data-model (cascade + `SetCurrentTurnFighter`); `RedoAsync` clears sub-commands and re-executes

### Tests (all parallelizable with each other, depend on their respective implementations)

- [X] T018 [P] [US1] Create `tests/Business/DnDActionsTests/TurnActions/EndTurnCommandHandlerTests.cs` — `ExecuteTests`: verify `CloseBlockCommand` dispatched as sub-command
- [X] T019 [P] [US1] Create `tests/Business/DnDActionsTests/TurnActions/StartNextRoundCommandHandlerTests.cs` — `ExecuteTests`: `PreviousRound` set, `SetCurrentRound` called, log sub-command dispatched; `UndoTests`: round restored
- [X] T020 [P] [US1] Create `tests/Business/DnDActionsTests/TurnActions/StartTurnCommandHandlerTests.cs` — `ExecuteTests`: `OpenBlockCommand` dispatched with correct fighter name
- [X] T021 [US1] Create `tests/Business/DnDActionsTests/TurnActions/StartNextTurnCommandHandlerTests.cs` — `ExecuteTests`: first turn (Initialize called, no EndTurn, StartNextRound dispatched), subsequent turn (EndTurn dispatched, round-end triggers StartNextRound, non-round-end skips it), `PreviousFighterId` stored; `UndoTests`: `SetCurrentTurnFighter` called; `RedoTests`: sub-commands cleared, ExecuteAsync re-runs

**Checkpoint**: Full turn-advance mechanic works. Current turn fighter tracked. Round increments. Undo/redo verified by tests.

---

## Phase 4: User Story 2 — Display Current Round and Turn Information (Priority: P1)

**Goal**: The bottom-right of the fight screen shows current round, current turn fighter name, and the "Start Combat"/"Next Turn" button. Display updates reactively when `ICombatTurnService.OnChanged` fires.

**Independent Test**: Open the fight screen — bottom-right shows "—" (not started). Press "Start Combat" → shows "Round 1 — Fighter A's turn". Press "Next Turn" → updates to Fighter B.

- [X] T022 [P] [US2] Create `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor` — `<MudButton>` with label binding; round display; turn display; inherits `StylableComponentBase`
- [X] T023 [P] [US2] Create `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.cs` — injects `ICombatTurnService`, `IFightContext`, `IUndoableMediator`; subscribes to `OnChanged`; computes `ButtonLabel`, `RoundText`, `TurnText`, `IsDisabled`; `OnButtonClick` sends `new StartNextTurnCommand()`; disposes subscriptions
- [X] T024 [P] [US2] Create `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.css` — scoped styles for the bottom-right panel layout
- [X] T025 [US2] Modify `src/Components/DndUi.Shared/Components/Pages/FightPage.razor` — replace `<div style="grid-row:2; grid-column:2;">general fight infos</div>` with `<CombatStatusComponent style="grid-row:2; grid-column:2;" />`

**Checkpoint**: Bottom-right panel renders correctly. Button triggers `StartNextTurnCommand`. Round/turn labels update reactively.

---

## Phase 5: User Story 3 — Current Turn Fighter Auto-Highlighted in UI (Priority: P1)

**Goal**: When a turn begins the new fighter is automatically highlighted/selected in the fight interface. The selected fighter is pure UI state managed by `FightPage` via a `CascadingValue`, driven by `ICombatTurnService.OnChanged`.

**Independent Test**: Advance through two turns — verify the fighter tile for the current turn fighter shows a highlighted state each time.

> **Delivered by**:
> - `FightPage.razor.cs` subscribes to `ICombatTurnService.OnChanged` and sets `_selectedFighter = CurrentTurnFighter`
> - `FightPage.razor` wraps content in `<CascadingValue Value="_selectedFighter" Name="SelectedFighter">`
> - `FightingCharacterTile.razor.cs` receives `[CascadingParameter(Name = "SelectedFighter")]` and computes `_isSelected`
> - `MartialAttackSelectorComponent.razor.cs` receives `[CascadingParameter(Name = "SelectedFighter")]` for the actions panel
> - `ActiveFighter`, `SetActiveFighter`, `ClearActiveFighter`, `OnActiveFighterChanged` removed from `IFightContext`/`FightContext`

**Checkpoint**: Fighter tile highlights match the current turn fighter on each turn advance. User can also click tiles to change the selected fighter independently.

---

## Phase 6: User Story 4 — Character Turn as Log Grouping (Priority: P2)

**Goal**: The fight log groups all actions under the current turn fighter's heading. Attacks appear as indented scopes within the turn block, not as independent top-level sections.

**Independent Test**: Start a fight, execute one attack during Fighter A's turn, advance to Fighter B's turn, execute one attack. Verify the log shows two top-level turn blocks, each with its attack indented beneath it.

> **Note**: Turn block open/close is already implemented in US1 (T015 `StartTurnCommandHandler` → `OpenBlockCommand`, T011 `EndTurnCommandHandler` → `CloseBlockCommand`). The only new task for US4 is migrating the attack handler from block to scope.

- [X] T026 [US4] Modify `src/Business/DnDActions/MartialAttackActions/ExecuteMartialAttack/ExecuteMartialAttackCommandHandler.cs` — `OpenAttackLog`: replace `SendAsSubCommandAsync(new OpenBlockCommand(...))` with `SendAsSubCommandAsync(new OpenScopeCommand())`; `CloseAttackLog`: replace `SendAsSubCommandAsync(new CloseBlockCommand())` with `SendAsSubCommandAsync(new CloseScopeCommand())`

**Checkpoint**: Attack log entries appear nested under the current turn fighter's block. No top-level attack sections exist.

---

## Phase 7: Polish & Cross-Cutting Concerns

- [X] T027 [P] Full solution build check — verify no compiler errors after all changes (`DnDFightTool.slnx`)
- [X] T028 [P] Run all tests — verify no regressions in existing test suite

---

## Dependencies

```
T001 (DI)
T002 (ICombatTurnService) ←─── T003 (CombatTurnService)
T005+T006 (IDnDLogService changes) ←─ T007 (CloseBlockCommand ClosedBlockId) ←─ T008 (CloseBlockCommandHandler)
T005+T006 ←─ T009 (OpenBlockCommandHandler UndoAsync)

Phase 2 complete
  ├─ T010+T011 (EndTurnCommand/Handler) ─┐
  ├─ T012+T013 (StartNextRoundCommand/Handler) ─┤
  ├─ T014+T015 (StartTurnCommand/Handler) ─┤── T016+T017 (StartNextTurnCommand/Handler)
  └─ T002 ────────────────────────────────┘
       ├─ T018 (EndTurnCommandHandlerTests)
       ├─ T019 (StartNextRoundCommandHandlerTests)
       ├─ T020 (StartTurnCommandHandlerTests)
       └─ T021 (StartNextTurnCommandHandlerTests)

T022+T023+T024 (CombatStatusComponent) ←─ T025 (FightPage.razor)
T026 (ExecuteMartialAttack migration) — independent of US1/US2/US3
T027+T028 — after all above
```

## Parallel Execution Opportunities

**Phase 2 (all parallel)**:  T002, T004, T005+T006 can start simultaneously.

**Phase 3 sub-commands (all parallel with each other)**:  T010, T011, T012, T013, T014, T015 — all different files, no inter-dependency.

**Phase 3 tests (all parallel with each other)**:  T018, T019, T020 — once their respective handlers exist.

**Phase 4 (T022, T023, T024 parallel)**:  Component files are independent; T025 waits for T022.

**Phase 6 independent of Phase 4**:  T026 can start as soon as Phase 2 is complete, no dependency on the UI component.

## Implementation Strategy

**MVP Scope (deliver first)**: Phase 1 → Phase 2 → Phase 3 → Phase 4.
Covers all three P1 user stories: turn advancement (US1), display (US2), auto-selection (US3).

**P2 delivery**: Phase 6 (US4 — log grouping) can follow independently once Phase 2 is complete.

## Summary

| Metric | Value |
|---|---|
| Total tasks | 28 |
| Phase 1 (Setup) | 1 |
| Phase 2 (Foundational) | 8 |
| Phase 3 (US1 — Turn Advance) | 12 |
| Phase 4 (US2 — Display) | 4 |
| Phase 5 (US3 — Auto-Select) | 0 new files |
| Phase 6 (US4 — Log Grouping) | 1 |
| Phase 7 (Polish) | 2 |
| New source files | 14 |
| Modified source files | 8 |
| New test files | 4 |
