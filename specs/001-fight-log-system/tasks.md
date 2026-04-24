# Tasks: DnD Log System

**Input**: Design documents from `/specs/001-fight-log-system/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, quickstart.md

**Tests**: Tests are included — the spec explicitly mentions testing (NUnit 4 + FluentAssertions 7 + FakeItEasy 9).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the new `Logs` domain project, wire it into the solution, and register DI.

- [x] T001 Create `src/Domain/Logs/Logs.csproj` targeting `net10.0` with no external dependencies and `RootNamespace` set to `DnDFightTool.Domain.Logs`
- [x] T002 [P] Create `LogColorToken` enum with 20 values in `src/Domain/Logs/LogColorToken.cs`
- [x] T003 [P] Create `LogEntry` record (Id, Content, IndentLevel) in `src/Domain/Logs/LogEntry.cs`
- [x] T004 [P] Create `LogBlock` class (Id, Name, Entries list) in `src/Domain/Logs/LogBlock.cs`
- [x] T005 Create `IDnDLogService` interface (Blocks, OnChanged, OpenBlock, CloseBlock, OpenScope, CloseScope, AddEntry, Hide, Show, IsHidden, Clear) in `src/Domain/Logs/IDnDLogService.cs`
- [x] T006 Create `DnDLogService` implementation with `HashSet<Guid>` for hidden state in `src/Domain/Logs/DnDLogService.cs`
- [x] T007 Create IoC registration `RegisterLogsServices()` as singleton in `src/Domain/Logs/IoC/ServiceCollectionExtensions.cs`
- [x] T008 Add `Logs.csproj` to solution file `DnDFightTool.slnx`
- [x] T009 Register `IDnDLogService` in `src/Components/DndUi/MauiProgram.cs` by calling `RegisterLogsServices()`
- [x] T010 [P] Register `IDnDLogService` in `src/Components/DndUi.Web/Program.cs` by calling `RegisterLogsServices()`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Create `WriteLogCommand` and its handler, plus the test project. All user stories depend on this.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T011 Add project reference from `DnDActions.csproj` to `Logs.csproj` in `src/Business/DnDActions/DnDActions.csproj`
- [x] T012 Create `WriteLogCommand` (CommandBase, Content string, LogEntryId Guid?) in `src/Business/DnDActions/LogActions/WriteLog/WriteLogCommand.cs`
- [x] T013 Create `WriteLogCommandHandler` (Execute → AddEntry + store GUID, Undo → Hide, Redo → Show) in `src/Business/DnDActions/LogActions/WriteLog/WriteLogCommandHandler.cs`
- [x] T053 Create structural log sub-commands and their handlers (Execute → call service, Undo = no-op):
  - `OpenBlockCommand` / `OpenBlockCommandHandler` in `src/Business/DnDActions/LogActions/OpenBlock/`
  - `CloseBlockCommand` / `CloseBlockCommandHandler` in `src/Business/DnDActions/LogActions/CloseBlock/`
  - `OpenScopeCommand` / `OpenScopeCommandHandler` in `src/Business/DnDActions/LogActions/OpenScope/`
  - `CloseScopeCommand` / `CloseScopeCommandHandler` in `src/Business/DnDActions/LogActions/CloseScope/`
  > Block/scope lifecycle is managed via these sub-commands so the entire log lifecycle is in the mediator undo tree. No business handler injects `IDnDLogService` directly.
- [x] T014 Create test project `tests/Domain/LogsTests/LogsTests.csproj` referencing `Logs.csproj`, NUnit 4, FluentAssertions 7
- [x] T015 Add `LogsTests.csproj` to solution file `DnDFightTool.slnx`
- [x] T016 Create `DnDLogServiceTests` covering AddEntry, Hide, Show, IsHidden, OpenBlock/CloseBlock, OpenScope/CloseScope, Clear, OnChanged in `tests/Domain/LogsTests/DnDLogServiceTests.cs`
- [x] T017 Create `WriteLogCommandHandlerTests` covering Execute, Undo (hides), Redo (shows) in `tests/Business/DnDActionsTests/LogActions/WriteLogCommandHandlerTests.cs`

**Checkpoint**: Foundation ready — domain log service, write-log sub-command, and tests all in place

---

## Phase 3: User Story 1 — Command Handler Emits Log Entries (Priority: P1) 🎯 MVP

**Goal**: Any command handler can produce log entries via write-log sub-commands through `IDnDLogService`.

**Independent Test**: Execute `WriteLogCommand` as a sub-command and verify entries are created with unique IDs, preserving creation order.

### Implementation for User Story 1

- [x] T018 [US1] Add explicit project reference from `FightBlazorComponents.csproj` to `Logs.csproj` in `src/UI/FightBlazorComponents/FightBlazorComponents.csproj` (explicit dep, even though transitive path exists via DnDActions)

**Checkpoint**: US1 is implicitly satisfied by Phase 2 (WriteLogCommand + handler). T018 wires the UI project for subsequent stories.

---

## Phase 4: User Story 2 — Grouped Visual Blocks (Priority: P1) 🎯 MVP

**Goal**: Related log entries are visually grouped into blocks with spacing and hover highlighting.

**Independent Test**: Open two blocks, write entries, verify each block renders separately with spacing and hover highlight.

### Implementation for User Story 2

- [x] T019 [US2] Create `LogBlockComponent.razor` rendering a single block's entries with block-level hover highlight in `src/UI/FightBlazorComponents/Log/LogBlockComponent.razor`
- [x] T020 [P] [US2] Create `LogBlockComponent.razor.cs` code-behind inheriting `StylableComponentBase` with `[Parameter] LogBlock Block` in `src/UI/FightBlazorComponents/Log/LogBlockComponent.razor.cs`
- [x] T021 [P] [US2] Create `LogBlockComponent.razor.css` with block spacing (margin), hover highlight (subtle background) in `src/UI/FightBlazorComponents/Log/LogBlockComponent.razor.css`
- [x] T022 [US2] Create `DnDLogComponent.razor` rendering `LogService.Blocks` as a scrollable list of `LogBlockComponent` in `src/UI/FightBlazorComponents/Log/DnDLogComponent.razor`
- [x] T023 [P] [US2] Create `DnDLogComponent.razor.cs` code-behind inheriting `StylableComponentBase`, injecting `IDnDLogService`, subscribing to `OnChanged`, implementing `IDisposable` in `src/UI/FightBlazorComponents/Log/DnDLogComponent.razor.cs`
- [x] T024 [P] [US2] Create `DnDLogComponent.razor.css` with scrollable container styling in `src/UI/FightBlazorComponents/Log/DnDLogComponent.razor.css`

**Checkpoint**: Blocks render with spacing and hover; log panel shows all blocks.

---

## Phase 5: User Story 3 — Rich Text Formatting (Priority: P1) 🎯 MVP

**Goal**: Log entries display bold text, semantic colors, and hover tooltips from BBCode-like tags.

**Independent Test**: Create entries with `[b]`, `[c:fire]`, `[hover:]` tags and verify correct HTML rendering.

### Implementation for User Story 3

- [x] T025 [US3] Create `LogToken` type hierarchy (TextToken, BoldStart, BoldEnd, ColorStart, ColorEnd, HoverStart, HoverEnd) in `src/UI/FightBlazorComponents/Log/Parsing/LogToken.cs`
- [x] T026 [US3] Create `LogTokenParser.Parse(string) → IReadOnlyList<LogToken>` single-pass tokenizer validating color tokens against `LogColorToken` enum in `src/UI/FightBlazorComponents/Log/Parsing/LogTokenParser.cs`
- [x] T027a [US3] Create test project `tests/UI/FightBlazorComponentsTests/FightBlazorComponentsTests.csproj` referencing `FightBlazorComponents.csproj`, NUnit 4, FluentAssertions 7
- [x] T027b [US3] Add `FightBlazorComponentsTests.csproj` to solution file `DnDFightTool.slnx`
- [x] T027 [US3] Create `LogTokenParserTests` covering bold, color, hover, nesting, malformed tags, unknown color tokens rendered as literal text in `tests/UI/FightBlazorComponentsTests/Log/LogTokenParserTests.cs`
- [x] T028 [US3] Create `LogEntryComponent.razor` rendering parsed tokens as HTML spans (bold → `<strong>`, color → `<span style="color:var(...)">`, hover → `<span title="...">`) in `src/UI/FightBlazorComponents/Log/LogEntryComponent.razor`
- [x] T029 [P] [US3] Create `LogEntryComponent.razor.cs` code-behind inheriting `StylableComponentBase` with `[Parameter] LogEntry Entry` and `[Parameter] IDnDLogService LogService` for visibility check in `src/UI/FightBlazorComponents/Log/LogEntryComponent.razor.cs`
- [x] T030 [P] [US3] Create `LogEntryComponent.razor.css` with indentation styling (padding-left based on IndentLevel) in `src/UI/FightBlazorComponents/Log/LogEntryComponent.razor.css`
- [x] T031 [US3] Wire `LogEntryComponent` into `LogBlockComponent.razor` to render each entry in the block in `src/UI/FightBlazorComponents/Log/LogBlockComponent.razor`

**Checkpoint**: Entries render with formatting; parser handles all tag types and edge cases.

---

## Phase 6: User Story 4 — Indented / Nested Scopes (Priority: P2)

**Goal**: Log entries created within an open scope render indented relative to the parent scope.

**Independent Test**: Open a scope, write entries, close scope, write more entries; verify indentation levels differ.

### Implementation for User Story 4

- [x] T032 [US4] Add tests for scope-based indentation in `DnDLogServiceTests`: OpenScope increments indent, CloseScope decrements, entries capture current indent level in `tests/Domain/LogsTests/DnDLogServiceTests.cs`

**Checkpoint**: US4 is implicitly supported by `DnDLogService.OpenScope/CloseScope` (Phase 2) and `LogEntryComponent` indentation CSS (T030). T032 adds explicit test coverage.

---

## Phase 7: User Story 5 — Undo Hides Log Entries, Redo Restores Them (Priority: P2)

**Goal**: Undoing a command hides its log entries; redoing restores them in original order.

**Independent Test**: Execute a command with write-log sub-commands, undo it, verify entries are hidden; redo, verify entries reappear.

### Implementation for User Story 5

- [x] T033a [US5] Update `LogBlockComponent.razor` to filter out entries where `LogService.IsHidden(entry.Id)` is true, and set a hidden CSS class (or skip rendering) when all entries in the block are hidden, in `src/UI/FightBlazorComponents/Log/LogBlockComponent.razor`
- [x] T033b [US5] Update `DnDLogComponent.razor` to skip rendering `LogBlockComponent` for blocks where all entries are hidden (computed via `block.Entries.All(e => LogService.IsHidden(e.Id))`) in `src/UI/FightBlazorComponents/Log/DnDLogComponent.razor`
- [x] T034 [US5] Add tests for undo/redo visibility in `WriteLogCommandHandlerTests`: Execute → entry visible, Undo → entry hidden, Redo → entry visible again in `tests/Business/DnDActionsTests/LogActions/WriteLogCommandHandlerTests.cs`

**Checkpoint**: Undo/redo toggles entry visibility correctly; UI filters hidden entries.

---

## Phase 8: User Story 6 — Semantic Color Tokens with Theme Support (Priority: P2)

**Goal**: All damage types and heal render in distinct, theme-aware colors.

**Independent Test**: Switch between light/dark themes and verify color token CSS variables produce readable, distinct colors.

### Implementation for User Story 6

- [x] T035 [US6] Create `log-colors.css` with `:root` (light) and `.mud-theme-dark` (dark) CSS custom properties for all 20 tokens in `src/Components/DndUi.Shared/wwwroot/css/log-colors.css`
- [x] T036a [US6] Add `<link rel="stylesheet" href="css/log-colors.css">` in `src/Components/DndUi/wwwroot/index.html` (MAUI host)
- [x] T036b [P] [US6] Add `<link rel="stylesheet" href="css/log-colors.css">` in `src/Components/DndUi.Web/wwwroot/index.html` (Web host)

**Checkpoint**: Color tokens render correctly in both light and dark themes.

---

## Phase 9: User Story 7 — Clear All Logs (Priority: P3)

**Goal**: Users can clear all log entries and blocks to start fresh.

**Independent Test**: Create entries, call `Clear()`, verify the log is empty.

### Implementation for User Story 7

- [x] T037 [US7] Add `Clear()` test in `DnDLogServiceTests`: verify all blocks, entries, and hidden state are removed, `OnChanged` fires in `tests/Domain/LogsTests/DnDLogServiceTests.cs`

**Checkpoint**: US7 is implemented by `DnDLogService.Clear()` in Phase 2 (T006). T037 adds explicit test coverage. UI clear button can be added later if needed.

---

## Phase 10: User Story 8 — Retrofit Existing Command Handlers (Priority: P3)

**Goal**: All 9 existing command handlers produce meaningful, formatted log entries.

**Independent Test**: Execute each handler and verify it produces at least one log entry with appropriate blocks, scopes, and formatting.

### Implementation for User Story 8

- [x] T038 [US8] Retrofit `ExecuteMartialAttackCommandHandler` to open/close a log block and scope via `OpenBlockCommand`/`CloseBlockCommand`/`OpenScopeCommand`/`CloseScopeCommand` sub-commands and write-log sub-commands for attack, hit roll, Hit!/Miss! in `src/Business/DnDActions/MartialAttackActions/ExecuteMartialAttack/ExecuteMartialAttackCommandHandler.cs`
  > Does NOT inject `IDnDLogService`. Uses structural sub-commands through the mediator, same as all other handlers.
- [x] T039 [US8] Add write-log sub-commands to `ApplyDamageRollResultsCommandHandler` with `[c:damageType]value damageType[/c]` formatting in `src/Business/DnDActions/DamageActions/ApplyDamageRollResults/ApplyDamageRollResultsCommandHandler.cs`
  > Does NOT inject `IDnDLogService` — log entries are emitted via `WriteLogCommand` sub-commands through the mediator only.
- [x] T040 [P] [US8] `TakeDamageCommandHandler` — no log entries added; it is a pure orchestrator delegating to `LooseHp`/`LooseTempHp` which handle their own logging in `src/Business/DnDActions/DamageActions/TakeDamage/TakeDamageCommandHandler.cs`
- [x] T041 [P] [US8] Add write-log sub-command to `LooseHpCommandHandler` with HP loss entry in `src/Business/DnDActions/HitPointActions/LooseHp/LooseHpCommandHandler.cs`
  > Does NOT inject `IDnDLogService`. Fighter resolved once from context; `HitPoints` accessed via `fighter.HitPoints`.
- [x] T042 [P] [US8] Add write-log sub-command to `LooseTempHpCommandHandler` with temp HP loss entry in `src/Business/DnDActions/HitPointActions/LooseTempHp/LooseTempHpCommandHandler.cs`
  > Does NOT inject `IDnDLogService`. Fighter resolved once from context.
- [x] T043 [P] [US8] Add write-log sub-command to `RegainHpCommandHandler` with `[c:heal]value HPs[/c]` entry in `src/Business/DnDActions/HitPointActions/RegainHp/RegainHpCommandHandler.cs`
  > Does NOT inject `IDnDLogService`. Fighter resolved once from context.
- [x] T044 [P] [US8] Add write-log sub-command to `RegainTempHpCommandHandler` with `[c:heal]value temp HPs[/c]` entry in `src/Business/DnDActions/HitPointActions/RegainTempHp/RegainTempHpCommandHandler.cs`
  > Does NOT inject `IDnDLogService`. Fighter resolved once from context.
- [x] T045 [P] [US8] Add write-log sub-command to `ApplyStatusCommandHandler` with status application entry in `src/Business/DnDActions/StatusActions/ApplyStatus/ApplyStatusCommandHandler.cs`
  > Does NOT inject `IDnDLogService`.
- [x] T046 [P] [US8] `TryApplyStatusCommandHandler` — no direct log entries; logging delegated entirely to `ApplyStatusCommand` sub-command in `src/Business/DnDActions/StatusActions/TryApplyStatus/TryApplyStatusCommandHandler.cs`
  > Does NOT inject `IDnDLogService`.
- [x] T047 [US8] Update existing handler tests in `tests/Business/DnDActionsTests/` to align constructor calls with final signatures (no `IDnDLogService` mock in any handler test)

**Checkpoint**: All 9 handlers produce formatted log entries via write-log sub-commands.

---

## Phase 11: User Story 9 — Developer Documentation (Priority: P3)

**Goal**: Updated instructions and a new skill file guide future handler authors to include logging.

**Independent Test**: Read the instructions and skill file; verify they contain correct API reference, tag syntax, token list, and example.

### Implementation for User Story 9

- [x] T048 [US9] Update `.github/instructions/commands.instructions.md` to add a logging section: handlers MUST produce log entries via write-log sub-commands, reference `IDnDLogService` API
- [x] T049 [US9] Create skill file `.github/skills/dnd-logging/SKILL.md` with full API reference, formatting tags, color tokens, block/scope patterns, and complete handler example

**Checkpoint**: Documentation is complete and reflects the finalized implementation.

---

## Phase 12: Polish & Cross-Cutting Concerns

**Purpose**: Final integration, fight page wiring, and validation.

- [x] T050 Replace fight log placeholder `<div>` with `<DnDLogComponent />` in `src/Components/DndUi.Shared/Components/Pages/FightPage.razor`
- [ ] T051 Run all tests and verify no regressions across the full solution
- [ ] T052 Validate quickstart.md code examples against the final implementation in `specs/001-fight-log-system/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 (T001-T007 for domain types) — BLOCKS all user stories. Includes T053 (structural sub-commands).
- **US1 (Phase 3)**: Depends on Phase 2 (WriteLogCommand exists)
- **US2 (Phase 4)**: Depends on Phase 2 (log service exists) — can start in parallel with US1
- **US3 (Phase 5)**: Depends on Phase 2 (log entries exist) — can start in parallel with US1/US2
- **US4 (Phase 6)**: Depends on Phase 2 (scope API exists)
- **US5 (Phase 7)**: Depends on Phase 2 (Hide/Show API) + US3 (entry rendering)
- **US6 (Phase 8)**: Depends on US3 (color tag rendering)
- **US7 (Phase 9)**: Depends on Phase 2 (Clear API exists)
- **US8 (Phase 10)**: Depends on Phase 2 + US3 (formatting) + US6 (colors). Can start after US3+US6.
- **US9 (Phase 11)**: Depends on ALL other stories being complete (documents the finalized API)
- **Polish (Phase 12)**: Depends on US2+US3 (components exist) for fight page wiring

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2
- **US2 (P1)**: Can start after Phase 2 — parallel with US1
- **US3 (P1)**: Can start after Phase 2 — parallel with US1, US2
- **US4 (P2)**: Can start after Phase 2 — parallel with US1-3
- **US5 (P2)**: Depends on US3 (entry rendering must exist for visibility filtering)
- **US6 (P2)**: Depends on US3 (color tokens must be rendered in components)
- **US7 (P3)**: Can start after Phase 2 — independent
- **US8 (P3)**: Depends on US3 + US6 (formatting + colors must work)
- **US9 (P3)**: LAST — depends on all other stories

### Within Each User Story

- Domain entities / service before UI components
- Parser before entry rendering component
- Components before fight page integration
- Implementation before documentation

### Parallel Opportunities

- T002, T003, T004 can all run in parallel (independent domain types)
- T009 and T010 can run in parallel (different composition roots)
- T019-T024 (US2) and T025-T031 (US3) can start in parallel after Phase 2
- T027a and T027b must be sequential (project then solution)
- T036a and T036b can run in parallel (different host projects)
- T040-T046 (US8 handler retrofits) can all run in parallel (different handler files)

---

## Parallel Example: Phase 1

```
# Launch domain types in parallel:
T002: Create LogColorToken enum in src/Domain/Logs/LogColorToken.cs
T003: Create LogEntry record in src/Domain/Logs/LogEntry.cs
T004: Create LogBlock class in src/Domain/Logs/LogBlock.cs
```

## Parallel Example: User Story 8

```
# Launch independent handler retrofits in parallel:
T040: TakeDamageCommandHandler
T041: LooseHpCommandHandler
T042: LooseTempHpCommandHandler
T043: RegainHpCommandHandler
T044: RegainTempHpCommandHandler
T045: ApplyStatusCommandHandler
T046: TryApplyStatusCommandHandler
```

---

## Implementation Strategy

### MVP First (User Stories 1-3 Only)

1. Complete Phase 1: Setup (T001-T010)
2. Complete Phase 2: Foundational (T011-T017)
3. Complete Phase 3: US1 (T018) — log emission wired
4. Complete Phase 4: US2 (T019-T024) — blocks render visually
5. Complete Phase 5: US3 (T025-T031) — rich text formatting works
6. Complete Phase 12: T050 — wire log component into fight page
7. **STOP and VALIDATE**: Test log creation, block rendering, and formatting end-to-end

### Incremental Delivery

1. Setup + Foundational → Domain log infrastructure ready
2. US1 + US2 + US3 → Log panel on fight page with formatted entries (MVP!)
3. US4 + US5 → Scopes (indentation) and undo/redo visibility
4. US6 → Theme-aware color tokens
5. US7 → Clear-all capability
6. US8 → All existing handlers produce log entries
7. US9 → Developer documentation (LAST)
8. Polish → Final validation pass
