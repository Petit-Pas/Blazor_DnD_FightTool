---
description: "Actionable, dependency-ordered task list for the Fighters Page feature"
---

# Tasks: Fighters Page

**Input**: Design documents from `/specs/004-fighters-page/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, quickstart.md

**Tests**: Test tasks ARE included — `data-model.md` and `quickstart.md` mandate handler tests with `Execute`/`Undo`/`Redo` nested classes (per repository convention) and `FightContext` test updates.

**Organization**: Tasks are grouped by user story (US1–US6) to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Maps to user stories from `spec.md` (US1–US6); Setup/Foundational/Polish phases have no story label
- All paths are repo-relative; absolute base is `d:\Code\Perso\DnDFightTool\`

## Path Conventions

- Domain: [src/Domain/Fight/](src/Domain/Fight/)
- Business commands: [src/Business/DnDActions/](src/Business/DnDActions/)
- Business queries: [src/Business/DnDQueries/](src/Business/DnDQueries/)
- UI query handlers: [src/UI/DnDQueryPrompter/](src/UI/DnDQueryPrompter/)
- Pages & nav: [src/Components/DndUi.Shared/Components/](src/Components/DndUi.Shared/Components/)
- Tests: [tests/](tests/)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create new feature folders. No new projects, no new dependencies (per plan.md Constitution Check).

- [x] T001 [P] Create folder [src/Business/DnDActions/FightActions/](src/Business/DnDActions/FightActions/) (with `AddToFight/` and `RemoveFromFight/` subfolders)
- [x] T002 [P] Create folder [src/Business/DnDQueries/FightQueries/](src/Business/DnDQueries/FightQueries/)
- [x] T003 [P] Create folder [src/UI/DnDQueryPrompter/FightQueries/](src/UI/DnDQueryPrompter/FightQueries/)
- [x] T004 [P] Create folder [tests/Business/DnDActionsTests/FightActions/](tests/Business/DnDActionsTests/FightActions/)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain & shared command surface changes that ALL user stories depend on. Per `data-model.md` §Domain Changes and `research.md` R-001, R-002, R-005, R-006.

**⚠️ CRITICAL**: No user-story phase can begin until this phase is complete and green.

- [x] T005 Add `Guid OriginalCharacterId { get; }` property and constructor parameter to `FightingCharacter` in [src/Domain/Fight/Characters/FightingCharacter.cs](src/Domain/Fight/Characters/FightingCharacter.cs); propagate through `FightingCharacter.Copy(IMapper)` mapping (R-001, FR-014)
  - [x] T005a If a Mapster `TypeAdapterConfig<FightingCharacter, …>` (or similar) is required for the new `OriginalCharacterId` to round-trip, update [src/Domain/Fight/Mapping/MappingConfigurations.cs](src/Domain/Fight/Mapping/MappingConfigurations.cs) accordingly. Verify first: `FightingCharacter.Copy(IMapper)` currently constructs the instance manually and only `mapper.Copy(_character)` traverses Mapster — so the explicit constructor call MUST be updated to pass `OriginalCharacterId`, AND any registered config that materializes a `FightingCharacter` (search for `FightingCharacter` references under `src/**/Mapping/**`) must include the new property. If no such config exists, document this as a no-op in the task notes. (depends on T005)
- [x] T006 Add `event EventHandler<FightingCharacter> OnFighterAdded` and `void Restore(FightingCharacter fighter)` to `IFightContext` in [src/Domain/Fight/IFightContext.cs](src/Domain/Fight/IFightContext.cs) (R-006, R-004)
- [x] T007 Update `FightContext.Add(Character)` in [src/Domain/Fight/FightContext.cs](src/Domain/Fight/FightContext.cs) to construct `FightingCharacter` with `originalCharacterId = character.Id` (NOT the cloned monster's regenerated id) and raise `OnFighterAdded` after insertion (depends on T005, T006)
- [x] T008 Update `FightContext.Remove(FightingCharacter)` in [src/Domain/Fight/FightContext.cs](src/Domain/Fight/FightContext.cs) to decrement `_monsterCountByOriginalId[fighter.OriginalCharacterId]` and remove the key when the count reaches 0; existing `OnFighterRemoved` raise stays (R-002, FR-011a) (depends on T005)
- [x] T009 Implement `FightContext.Restore(FightingCharacter)` in [src/Domain/Fight/FightContext.cs](src/Domain/Fight/FightContext.cs): insert into `_fighters[fighter.Id]`, re-increment `_monsterCountByOriginalId` for monsters, raise `OnFighterAdded` (R-004, FR-011a) (depends on T006, T007)
- [x] T010 Change `SetCurrentFighterCommand.FighterId` from `Guid` to `Guid?` in [src/Business/DnDActions/TurnActions/SetCurrentFighter/SetCurrentFighterCommand.cs](src/Business/DnDActions/TurnActions/SetCurrentFighter/SetCurrentFighterCommand.cs); verify handler already forwards to `ICombatTurnService.SetCurrentTurnFighter(Guid?)` (R-005, FR-011b)
- [x] T011 [P] Update `FightingCharacter` test factory/builder in [tests/Domain/DomainTestsUtilities/](tests/Domain/DomainTestsUtilities/) to populate `OriginalCharacterId` (default = wrapped character's `Id`; explicit override for same-kind monster scenarios) AND grep-and-fix every direct `new FightingCharacter(` call site across both `src/` and `tests/` to pass the new `originalCharacterId` argument (use a workspace-wide search for `new FightingCharacter(` — production call sites in `FightContext.Add` are covered by T007 but any other production or test instantiations must be updated here). T014's build gate (`dotnet build DnDFightTool.slnx`) is the catch-net for any miss. (R-010) (depends on T005)
- [x] T012 [P] Add `OnFighterAdded` raise assertion, `OriginalCharacterId` propagation assertion, counter-decrement-on-`Remove` assertion, and `Restore` behaviour assertions to [tests/Domain/FightTests/FightContextTests.cs](tests/Domain/FightTests/FightContextTests.cs) (depends on T007, T008, T009, T011)
- [x] T013 [P] Add a "null clears current fighter" test case to [tests/Business/DnDActionsTests/TurnActions/SetCurrentFighterCommandHandlerTests.cs](tests/Business/DnDActionsTests/TurnActions/SetCurrentFighterCommandHandlerTests.cs) (depends on T010)
- [x] T014 Build & test gate: `dotnet build DnDFightTool.slnx` and `dotnet test DnDFightTool.slnx` MUST be green before Phase 3 (depends on T005–T013)

**Checkpoint**: Domain extensions ready, counter logic centralized, `SetCurrentFighter` accepts null. User-story phases unblocked.

---

## Phase 3: User Story 1 — Add a Character/Monster to the Fight (Priority: P1) 🎯 MVP

**Goal**: User can add players and monsters to the fight from a new Fighters page; initiative is prompted (or inherited for same-kind monsters).

**Independent Test**: Open `/fighters`, click `+` on a player → initiative modal → confirm → player appears on right list, vanishes from left list. Click `+` on a goblin (none in fight) → modal. Click `+` on goblin again → no modal, goblin 2 inherits initiative. Cancel modal → no addition, no undo entry.

### Implementation for User Story 1

- [x] T015 [P] [US1] Create `InitiativeRollQuery : QueryBase<int>` (with `Guid CharacterId` constructor) in [src/Business/DnDQueries/FightQueries/InitiativeRollQuery.cs](src/Business/DnDQueries/FightQueries/InitiativeRollQuery.cs) (data-model §Business — Query)
- [x] T016 [P] [US1] Create `InitiativeRollQueryHandler : QueryHandlerBase<InitiativeRollQuery, int>` in [src/UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandler.cs](src/UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandler.cs); inject `IDialogServiceProvider`; open `InitiativeRollQueryHandlerModal` with `CharacterId` parameter; cancel → `QueryResponse<int>.Canceled(0)` (R-003, FR-007, FR-012) (depends on T015)
- [x] T017 [P] [US1] Create `InitiativeRollQueryHandlerModal.razor` + `.razor.cs` (+ optional empty `.razor.css`) in [src/UI/DnDQueryPrompter/FightQueries/](src/UI/DnDQueryPrompter/FightQueries/), mirroring `SaveRollResultQueryHandlerModal` (RollableDialogBase + d20 input + dex modifier label); on confirm → `DialogResult.Ok((int)_d20Roll.Result)` (R-003)
- [x] T018 [P] [US1] Create `AddToFightCommand : CommandBase` (`Guid SourceCharacterId`, `Guid? AddedFighterId`, `int? InitiativeRoll`, `bool InheritedInitiative`) in [src/Business/DnDActions/FightActions/AddToFight/AddToFightCommand.cs](src/Business/DnDActions/FightActions/AddToFight/AddToFightCommand.cs) (data-model §Business — Commands)
- [x] T019 [US1] Create `AddToFightCommandHandler` in [src/Business/DnDActions/FightActions/AddToFight/AddToFightCommandHandler.cs](src/Business/DnDActions/FightActions/AddToFight/AddToFightCommandHandler.cs); inject `IFightContext`, `ICharacterRepository`; implement Execute algorithm (D-004, D-005, FR-007, FR-008, FR-009): repo lookup → same-kind inheritance check via `OriginalCharacterId` → `InitiativeRollQuery` (cancel-before-mutation returns `Canceled`) → `_fightContext.Add(character)` → set `InitiativeRoll` on the new fighter → store `AddedFighterId` → emit `WriteLogCommand` sub-command per dnd-logging skill (R-011); Undo via `_fightContext.Remove(addedFighter)`; Redo via clear-sub-commands + re-execute (depends on T015, T018, T007, T011)
- [x] T020 [US1] Create `AddToFightCommandHandlerTests` in [tests/Business/DnDActionsTests/FightActions/AddToFightCommandHandlerTests.cs](tests/Business/DnDActionsTests/FightActions/AddToFightCommandHandlerTests.cs) with nested `ExecuteTests`/`UndoTests`/`RedoTests` covering: player prompt-and-add, monster first-of-kind prompt-and-add, monster same-kind inherits without prompt, monster different-kind prompts, prompt cancellation → `Canceled` + no mutation + no history, repository miss → `Failed`, undo removes & decrements counter, redo re-executes correctly (quickstart Step 4) (depends on T019)
- [x] T021 [P] [US1] Create `FightersPage.razor` in [src/Components/DndUi.Shared/Components/Pages/FightersPage.razor](src/Components/DndUi.Shared/Components/Pages/FightersPage.razor) with `@page "/fighters"`, two-column MudGrid layout, left panel (addable list with `+` MudIconButton), right panel (in-fight list with `-` MudIconButton); markup only — no `@code` block per `.github/instructions/blazor-components.instructions.md`
- [x] T022 [US1] Create `FightersPage.razor.cs` in [src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.cs](src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.cs); inherit `StylableComponentBase`-or-equivalent base if Class/Style exposed (otherwise plain `ComponentBase`); inject `IUndoableMediator`, `IFightContext`, `ICharacterRepository`, `IDialogService`, `IDialogServiceProvider`; in `OnInitializedAsync` call `DialogServiceProvider.SetDialogService(DialogService)`; subscribe to `IFightContext.OnFighterAdded` and `OnFighterRemoved` (handler calls `InvokeAsync(StateHasChanged)`); implement `IDisposable` to unsubscribe; implement `OnAddClick(Character)` → `_mediator.SendAsync(new AddToFightCommand(c.Id))`; sort right-list `OrderBy(FightingCharacter.InitiativeSortKey)` for FR-004 stability (D-008, D-010, FR-003, FR-012, FR-015, FR-017) (depends on T018, T021)
- [x] T023 [P] [US1] Create [src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.css](src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.css) for two-column layout and group-header spacing
- [x] T024 [US1] Add `<MudNavLink HRef="fighters" Icon="@Icons.Material.Filled.Groups">Fighters</MudNavLink>` to [src/Components/DndUi.Shared/Components/Layout/NavMenu.razor](src/Components/DndUi.Shared/Components/Layout/NavMenu.razor) (FR-001) (depends on T021)

**Checkpoint**: `/fighters` route loads; add flow works end-to-end; cancel discards. US1 acceptance scenarios 1–7 verifiable.

---

## Phase 4: User Story 2 — Remove a Fighter from the Fight (Priority: P1)

**Goal**: User can remove a fighter via `-` button (no confirmation). Active-fighter null-out works.

**Independent Test**: With ≥1 fighter in fight, click `-` → fighter disappears immediately. Removing the active fighter nulls FightDashboard's selection without crashing.

### Implementation for User Story 2

- [x] T025 [P] [US2] Create `RemoveFromFightCommand : CommandBase` (`Guid FighterId`, `FightingCharacter? RemovedFighter`) in [src/Business/DnDActions/FightActions/RemoveFromFight/RemoveFromFightCommand.cs](src/Business/DnDActions/FightActions/RemoveFromFight/RemoveFromFightCommand.cs) (data-model §Business — Commands)
- [x] T026 [US2] Create `RemoveFromFightCommandHandler` in [src/Business/DnDActions/FightActions/RemoveFromFight/RemoveFromFightCommandHandler.cs](src/Business/DnDActions/FightActions/RemoveFromFight/RemoveFromFightCommandHandler.cs); inject `IFightContext`, `ICombatTurnService`; Execute: capture `RemovedFighter` reference → if `_combatTurnService.CurrentTurnFighter?.Id == FighterId` send `SetCurrentFighterCommand(null)` as sub-command (D-009, R-005, FR-011b) → `_fightContext.Remove(fighter)` → emit `WriteLogCommand` sub-command (R-011); Undo: `_fightContext.Restore(RemovedFighter)` + base cascade (D-007, FR-011); Redo: clear-sub-commands + re-execute (depends on T009, T010, T025)
- [x] T027 [US2] Create `RemoveFromFightCommandHandlerTests` in [tests/Business/DnDActionsTests/FightActions/RemoveFromFightCommandHandlerTests.cs](tests/Business/DnDActionsTests/FightActions/RemoveFromFightCommandHandlerTests.cs) with nested `ExecuteTests`/`UndoTests`/`RedoTests` covering: non-active removal + counter decrement, active-fighter removal triggers `SetCurrentFighterCommand(null)` sub-command, undo restores same instance + re-increments counter, undo cascades to `SetCurrentFighter` sub-command undo, fighter id miss → `Failed`, redo removes again (quickstart Step 5) (depends on T026)
- [x] T028 [US2] Wire `OnRemoveClick(FightingCharacter)` in [src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.cs](src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.cs) → `_mediator.SendAsync(new RemoveFromFightCommand(f.Id))`; bind to `-` button in [src/Components/DndUi.Shared/Components/Pages/FightersPage.razor](src/Components/DndUi.Shared/Components/Pages/FightersPage.razor) (FR-006) (depends on T022, T025)

**Checkpoint**: US2 acceptance scenarios 1–5 verifiable. Active-fighter removal does not crash the (still-named) Fight page.

---

## Phase 5: User Story 3 — Undoable Add/Remove (Priority: P1)

**Goal**: Both `AddToFight` and `RemoveFromFight` integrate cleanly with the existing undo/redo controls (feature 003).

**Independent Test**: Add → undo (gone) → redo (back). Remove → undo (back, state preserved) → redo (gone). Adding a non-first-of-kind monster + undo only removes that one fighter.

### Implementation for User Story 3

- [x] T029 [US3] Verify `AddToFightCommandHandlerTests.UndoTests` (T020) covers: undo of non-first-of-kind add removes only the added fighter, leaves the original same-kind monster, decrements counter to the prior value; extend if missing
- [x] T030 [US3] Verify `RemoveFromFightCommandHandlerTests.UndoTests` (T027) covers: undo restores `InitiativeRoll`, current HP, statuses (i.e., the same `FightingCharacter` instance is returned to `_fighters`); extend if missing
- [x] T031 [US3] Manual smoke pass against the running app (`dotnet run --project src/Components/DndUi.Web/DndUi.Web.csproj`): exercise undo/redo for add and remove of player + monster (first-of-kind and same-kind) using the existing undo/redo buttons in `CombatStatusComponent`

**Checkpoint**: US3 acceptance scenarios 1–4 verifiable. No new UI surface — relies on feature 003 buttons.

---

## Phase 6: User Story 4 — Visual Grouping of Monsters and Characters (Priority: P2)

**Goal**: Both panels visually group Players and Monsters with section headers + dividers.

**Independent Test**: With a mix of players and monsters in the repository and the fight, both columns render distinct Player / Monster sections.

### Implementation for User Story 4

- [x] T032 [US4] In [src/Components/DndUi.Shared/Components/Pages/FightersPage.razor](src/Components/DndUi.Shared/Components/Pages/FightersPage.razor), render each panel as two grouped sections (Players, Monsters), each preceded by `<MudText Typo="Typo.subtitle2">` + `<MudDivider />` per R-007 / D-011; left list filters players already in fight (via `OriginalCharacterId` join with `FightContext.Fighters`) per D-010 / FR-003; monsters always shown
- [x] T033 [US4] In [src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.css](src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.css), add minimal spacing for group headers (no collapsibles) per R-007

**Checkpoint**: US4 acceptance scenarios 1–2 verifiable.

---

## Phase 7: User Story 5 — Rename Fight Page to FightDashboard (Priority: P2)

**Goal**: Existing Fight page is renamed to FightDashboard everywhere user-visible.

**Independent Test**: Nav menu shows "Fighters" + "FightDashboard"; no orphan "Fight". `/fight-dashboard` displays the previous Fight page content.

### Implementation for User Story 5

- [x] T034 [US5] Rename file [src/Components/DndUi.Shared/Components/Pages/FightPage.razor](src/Components/DndUi.Shared/Components/Pages/FightPage.razor) → `FightDashboardPage.razor`; change directive to `@page "/fight-dashboard"` (FR-002, D-013)
- [x] T035 [US5] Rename file [src/Components/DndUi.Shared/Components/Pages/FightPage.razor.cs](src/Components/DndUi.Shared/Components/Pages/FightPage.razor.cs) → `FightDashboardPage.razor.cs`; rename class `FightPage` → `FightDashboardPage` (depends on T034)
- [x] T036 [US5] Update [src/Components/DndUi.Shared/Components/Layout/NavMenu.razor](src/Components/DndUi.Shared/Components/Layout/NavMenu.razor): replace prior `Fight` link with `<MudNavLink HRef="fight-dashboard" Icon="@Icons.Material.Filled.AutoFixHigh">FightDashboard</MudNavLink>`; ensure no orphan label remains (FR-002) (depends on T024, T035)
- [x] T037 [US5] Verify `FightDashboardPage` renders gracefully when `_selectedFighter` is `null` (FR-011b, FR-016, D-013); add a guarded null-check only if a missing one is found — otherwise no change beyond rename
- [x] T038 [US5] `dotnet build DnDFightTool.slnx` to confirm no stale `FightPage` references remain in tests, layouts, or routes (depends on T034–T037)

**Checkpoint**: US5 acceptance scenarios 1–2 verifiable.

---

## Phase 8: User Story 6 — Remove AddToFight Button from Character List Editor (Priority: P2)

**Goal**: Single entry point for fight composition — the Fighters page.

**Independent Test**: Open the character list editor; no "Add to fight" button on any player or monster row.

### Implementation for User Story 6

- [x] T039 [US6] Remove both `<FightButton OnClick="() => AddToFight(...)" Size=Size.Small />` instances (Players panel + Monsters panel) from [src/Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor](src/Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor) (FR-013)
- [x] T040 [US6] Remove `AddToFight(Character)` method from [src/Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor.cs](src/Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor.cs); also remove the `[Inject] IFightContext FightContext` field if it has no remaining usage in the file (verify before deleting) (depends on T039)

**Checkpoint**: US6 acceptance scenarios 1–2 verifiable.

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup spanning all stories.

- [x] T041 Run `dotnet build DnDFightTool.slnx` — must be green
- [x] T042 Run `dotnet test DnDFightTool.slnx` — all green
- [x] T043 Walk through quickstart.md Verification Checklist end-to-end against the running web host: routes, prompt flow, same-kind inheritance, undo/redo, nav rename, removed button (quickstart §Verification Checklist)
- [x] T044 Confirm no new NuGet dependencies were introduced (Constitution gate, plan.md §Constitution Check)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — folder scaffolding only.
- **Foundational (Phase 2)**: Requires Setup. **Blocks every user-story phase** because it defines `OriginalCharacterId`, `OnFighterAdded`, `Restore`, `Remove`-counter-decrement, and `SetCurrentFighterCommand` nullable id.
- **User Story 1 (Phase 3, P1)**: Requires Foundational. MVP — first deployable increment.
- **User Story 2 (Phase 4, P1)**: Requires Foundational. Independent of US1 (different command, separate page wiring slot).
- **User Story 3 (Phase 5, P1)**: Validation pass on top of US1+US2. Tests live inside US1/US2 phases by handler convention; this phase only confirms coverage and does a manual smoke.
- **User Story 4 (Phase 6, P2)**: Requires US1's `FightersPage` skeleton (T021/T022). Pure rendering refinement.
- **User Story 5 (Phase 7, P2)**: Independent of US1–US4 except for the shared `NavMenu.razor` edit (T024 vs T036 — sequential).
- **User Story 6 (Phase 8, P2)**: Requires US1 fully shipped (Fighters page is the new single entry point).
- **Polish (Phase 9)**: Last — runs after all desired stories.

### Within-Story Dependencies (key chains)

- **US1**: T015 → T016 → T017 (query → handler → modal); T018 → T019 → T020 (command → handler → tests); T021 → T022 → T024 (page markup → code-behind → nav link).
- **US2**: T025 → T026 → T027 → T028.
- **US5**: T034 → T035 → T036 → T038.
- **US6**: T039 → T040.

### Cross-Story Note

- T024 (US1: add Fighters nav link) and T036 (US5: rename Fight → FightDashboard nav link) both edit `NavMenu.razor`. Sequence them: T024 first, T036 second. Not parallelizable.

### Parallel Opportunities

- All Phase 1 setup tasks T001–T004 run in parallel.
- Phase 2 parallel pairs: (T011, T012, T013) once their producers (T005–T010) are in.
- US1 parallel starting points: T015 ‖ T018 ‖ T021 ‖ T023 (different files, no inter-deps).
- US2 entry point T025 runs in parallel with US1 implementation if developer capacity permits (different folder, different command).
- US5 file renames T034/T035 can run in parallel with US6 T039/T040 (disjoint files) once Foundational is done.

---

## Parallel Example: User Story 1 kickoff

```text
# Once Foundational (Phase 2) is green, launch in parallel:
Task T015: Create InitiativeRollQuery in src/Business/DnDQueries/FightQueries/InitiativeRollQuery.cs
Task T018: Create AddToFightCommand in src/Business/DnDActions/FightActions/AddToFight/AddToFightCommand.cs
Task T021: Scaffold FightersPage.razor markup in src/Components/DndUi.Shared/Components/Pages/FightersPage.razor
Task T023: Create FightersPage.razor.css in src/Components/DndUi.Shared/Components/Pages/FightersPage.razor.css
```

---

## Implementation Strategy

### MVP Scope (US1 + US2 + US3)

The three P1 stories are tightly coupled (compose, decompose, undo) and form the minimum shippable feature.

1. Phase 1 → Phase 2 (foundation green).
2. Phase 3 (US1) → first walkable demo (`/fighters` add flow + manual undo).
3. Phase 4 (US2) → remove flow.
4. Phase 5 (US3) → confirm undo/redo for both directions.
5. **Stop & validate**: spec.md acceptance scenarios for US1–US3.

### Incremental Delivery After MVP

6. Phase 6 (US4) — visual polish.
7. Phase 7 (US5) — rename for clarity.
8. Phase 8 (US6) — remove duplicate entry point.
9. Phase 9 — final validation.

Each phase preserves green build & tests; commit at every checkpoint.

---

## Notes

- Tests live within their owning user-story phase per repository convention (`Execute`/`Undo`/`Redo` nested classes — see `.github/instructions/tests.instructions.md`).
- No `contracts/` directory: this feature exposes no public API surface.
- `IUndoableMediator` auto-discovers handlers — **no DI registration tasks** are required for the new command/query handlers (plan.md §Constitution Check; quickstart §DI Notes).
- `[P]` tasks: confirm they touch disjoint files; otherwise sequence them.
- Avoid: breaking `FightingCharacter` callers in test factories (T011 protects against this), introducing logic into `.razor` files (T021 keeps markup-only), bypassing the mediator for fight mutations (T039/T040 enforce single-entry).
