# Tasks: Undo/Redo Buttons

**Input**: Design documents from `/specs/003-undo-redo-buttons/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, quickstart.md

**Tests**: Not explicitly requested in spec. Optional bUnit tests included as a final phase task.

**Organization**: Tasks grouped by user story. US3 (visual feedback) is fully satisfied by US1+US2 implementation — no separate phase needed.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Exact file paths included in descriptions

---

## Phase 1: Setup (NuGet Prerequisite)

**Purpose**: Update the UndoableMediator library to expose the required events

- [X] T001 [P] Update UndoableMediator PackageReference from `2.0.0-alpha2` to `2.0.0-alpha3` in `src/Business/DnDActions/DnDActions.csproj`
- [X] T002 [P] Update UndoableMediator PackageReference from `2.0.0-alpha2` to `2.0.0-alpha3` in `src/Business/DnDQueries/DnDQueries.csproj`
- [X] T003 [P] Update UndoableMediator PackageReference from `2.0.0-alpha2` to `2.0.0-alpha3` in `src/Infrastructure/Extensions/Extensions.csproj`
- [X] T004 [P] Update UndoableMediator PackageReference from `2.0.0-alpha2` to `2.0.0-alpha3` in `src/Components/DndUi/DndUi.csproj`
- [X] T005 [P] Update UndoableMediator PackageReference from `2.0.0-alpha2` to `2.0.0-alpha3` in `src/Components/DndUi.Web/DndUi.Web.csproj`
- [X] T006 Verify solution builds successfully: `dotnet build DnDFightTool.slnx`

**Checkpoint**: UndoableMediator updated — `OnCommandExecuted`, `OnCommandUndone`, `OnCommandRedone` events available on `IUndoableMediator`

---

## Phase 2: User Story 1 — Undo Last Action (Priority: P1) 🎯 MVP

**Goal**: DM can click an undo button to revert the last executed command

**Independent Test**: Perform any combat action (e.g., advance turn) → click undo button → game state reverts

### Implementation for User Story 1

- [X] T007 [US1] Add event subscriptions (`OnCommandExecuted`, `OnCommandUndone`, `OnCommandRedone`) in `OnInitialized()` and unsubscribe in `Dispose()` in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.cs`
- [X] T008 [US1] Add `CanUndo` computed property (`_mediator.HistoryLength > 0`) in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.cs`
- [X] T009 [US1] Add `OnUndoClick()` async handler calling `_mediator.UndoLastCommandAsync()` in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.cs`
- [X] T010 [US1] Add undo `MudIconButton` (Icons.Material.Filled.Undo, Disabled=!CanUndo, OnClick=OnUndoClick) to the markup in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor`
- [X] T011 [US1] Add button-row container styling (top-right positioning within panel) in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.css`

**Checkpoint**: Undo button is visible, greyed out when history is empty, and reverts last command when clicked

---

## Phase 3: User Story 2 — Redo Undone Action (Priority: P2)

**Goal**: DM can click a redo button to re-apply the last undone command

**Independent Test**: Perform action → undo → click redo button → game state restored

### Implementation for User Story 2

- [X] T012 [US2] Add `CanRedo` computed property (`_mediator.RedoHistoryLength > 0`) in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.cs`
- [X] T013 [US2] Add `OnRedoClick()` async handler calling `_mediator.RedoLastUndoneCommandAsync()` in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.cs`
- [X] T014 [US2] Add redo `MudIconButton` (Icons.Material.Filled.Redo, Disabled=!CanRedo, OnClick=OnRedoClick) to the markup in `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor`

**Checkpoint**: Both undo and redo buttons work, disabled states update reactively via event subscriptions (US3 satisfied implicitly)

---

## Phase 4: Polish & Verification

**Purpose**: Validate all stories work together and the feature is complete

- [X] T015 Verify solution builds: `dotnet build DnDFightTool.slnx`
- [X] T016 Run existing tests: `dotnet test DnDFightTool.slnx`
- [ ] T017 Manual verification per quickstart.md: both buttons greyed initially → perform action → undo active → click undo → redo active → click redo → state restored
- [ ] T018 (Optional) Add bUnit test class verifying button disabled states and click handlers in `tests/UI/FightBlazorComponentsTests/CombatStatus/CombatStatusComponentTests.cs`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately. BLOCKS all other phases.
- **User Story 1 (Phase 2)**: Depends on Phase 1 completion.
- **User Story 2 (Phase 3)**: Depends on Phase 2 (shares the same file, event subscriptions from US1 are reused).
- **Polish (Phase 4)**: Depends on Phases 2 and 3 completion.

### User Story Dependencies

- **User Story 1 (P1)**: Blocked by Phase 1 (NuGet update). No other story dependency.
- **User Story 2 (P2)**: Depends on US1 event subscription infrastructure (T007). Shares the same code-behind file.
- **User Story 3 (P3)**: Fully satisfied by US1 + US2 implementation — the `Disabled` parameter on `MudIconButton` and event-driven `StateHasChanged` provide the visual feedback. No separate tasks needed.

### Parallel Opportunities

- **Phase 1**: All five csproj updates (T001–T005) can be applied in parallel.
- **Phase 2**: T008 and T009 can be written in parallel (different concerns in the same file, but no conflict). T010 and T011 are in different files and can be parallel.
- **Phase 3**: T012 and T013 can be written together (same file, adjacent code).

---

## Parallel Example: Phase 1

```text
# All NuGet version bumps at once:
T001: Update DnDActions.csproj
T002: Update DnDQueries.csproj
T003: Update Extensions.csproj
T004: Update DndUi.csproj
T005: Update DndUi.Web.csproj
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: NuGet update
2. Complete Phase 2: Undo button (US1)
3. **STOP and VALIDATE**: Undo works independently
4. Proceed to US2 (redo)

### Incremental Delivery

1. Phase 1 → NuGet prerequisite met
2. Phase 2 → Undo works → Immediately useful for DM mistake recovery
3. Phase 3 → Redo works → Full undo/redo experience
4. Phase 4 → Polish, tests, verification

---

## Notes

- All source changes are in **3 files** of one existing component + **5 csproj** version bumps
- No new projects, no new domain types, no new DI registrations
- The `HandleStateChanged` method already exists in the component — reuse it for mediator event subscriptions
- US3 (visual feedback) has no dedicated tasks because `MudIconButton.Disabled` + event-driven re-render covers it entirely via US1 + US2
