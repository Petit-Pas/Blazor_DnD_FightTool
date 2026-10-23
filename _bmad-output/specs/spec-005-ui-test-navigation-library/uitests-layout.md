# /uitests layout

How `/uitests` is split into projects, and where each file goes. A reader finds a component's typed object and its UI test by knowing only where the component lives in `src/`.

## Projects

| Kind | Path | Holds |
|---|---|---|
| Framework | `uitests/Framework/UiTestFramework.csproj` | Plain library, framework pieces only: `ApplicationFixture`, `IsolatedScenarioFixture`, `SequentialScenarioFixture`, `AssemblyFixture`, `AssemblyInfo`, `Pages/PageObject`, `Components/ComponentObject`, `Dialogs/DialogSeam`, `Extensions/`. No typed object, no test. `ApplicationFixture` still composes `DndUi.Web` and resets domain state; its split is deferred. |
| Testable components | `uitests/TestableComponents/{src layer}/Testable{OriginProject}/` | Plain library of typed objects (page, component and dialog objects) for the components of one `src/` project. |
| Tests | `uitests/Tests/{src layer}/{OriginProject}UiTests/` | NUnit test project with the UI tests of one `src/` project's components. |
| Framework tests | `uitests/Tests/Framework/UiTestFrameworkUiTests/Meta/` | Tests that validate the framework itself. |
| Scratch | `uitests/Scratch/` | Agent throwaway scenarios (CAP-7); see its README. `*.cs`/`*.png` gitignored. |

Testable and test projects are created only once they hold something.

## Placement rules

1. **Project path** mirrors the origin project's `src/` path exactly, `src/UI` layer included.
   - `src/UI/CharacterSheetBlazorComponents` → `uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents` and `uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests`
   - `src/Components/DndUi.Shared` → `uitests/TestableComponents/Components/TestableDndUi.Shared` and `uitests/Tests/Components/DndUi.SharedUiTests`
2. **File path** inside either project is the component's exact path inside its origin project.
3. **Typed object** file and class: `Testable{Component}.cs` / `Testable{Component}` — one per component, dialogs included (e.g. `TestableInitiativeInputDialog`).
4. **Test** file: `{TestedComponent}UiTests.cs` — exactly one file per tested component.
5. A test touching several components is placed by its **entry** page or component (the one it navigates to first or primarily asserts on).

## Test file shape

The file holds a `public static class {TestedComponent}UiTests` container with no tests directly on it. Each **business capability** is a nested fixture (for example `CreateCharacters`, `DeleteCharacters`, `EditCharacters`). Creation includes the Duplicate button; deletion has independently arranged targets, not a dependency on a preceding UI creation test. A row's clone provenance alone does not justify another UI case.

When player and monster controls occupy different places in the UI: test them with separately named methods and direct calls to their respective typed controls. Do not combine them behind `CharacterType` parameters or routing helpers. Likewise, Add and Duplicate cancellation paths use separate named tests, not a `duplicated` boolean. This does not require duplicating every generic editor-cancellation or domain-input case.

Put unconditional common arrangement in NUnit `[SetUp]`: shared seeding and page-object construction, plus navigation when common to every test at that point. Each nested fixture derives directly from `IsolatedScenarioFixture` or `SequentialScenarioFixture` and owns its `[SetUp]`; do not add a local abstract base fixture (existing `DashboardFixture` and `CharacterLibraryFixture` are tracked for removal in DW-021). Keep different turn counts, undo state and per-test seed data explicit. Every test reaches its page the way a user does: start from a page object's `GoToAsync` and click through the production controls (for example `CharacterList.EditPlayer(name)` → `OpenAttacks()`). Write that seeding and navigation in the test or its `[SetUp]`; do not hide it in a static helper such as the former `OpenCharacter(page, baseUrl, services, ...)`, introduce a general flow layer, or use setup to hide the action under test.

A control that navigates returns the typed object of its destination: the list page's Edit control (`TestableCharacterListEditorPage.EditPlayer(name)`) returns `TestableCharacterEditorPage`. Character editor tests seed through `ICharacterRepository` in their own `[SetUp]` and open the editor that way; there is no separate tools object or shared base fixture, so each fixture picks its own base class.

Test bodies pass typed objects the strings the user sees, written as literals (`"STR"`, `"Athletics"`, `"Fire"`, `"Expertise"`). Compute a rendered string from a domain value only when the test iterates over domain values, as the every-damage-type rendering test does.

Seed every starting value a test acts on or asserts against (for example `SeededStrengthScore`); never rely on factory or constructor defaults. A rendering test of a two-state control checks both states (for example STR mastered, DEX not).

### Locators and accessible names

Typed objects find elements by role + accessible name only. Do not anchor on layout or utility CSS classes, visible title text, or DOM position (`.First`, `..` parent hops). When a control or region has no usable accessible name, add `role`/`aria-label` to the production markup (pre-approved; list it in the story's Tasks), for example `role="region" aria-label="Skills"` on an editor panel or `role="group" aria-label="STR ability"` on a row. `data-testid` still needs approval. Older locators that break this rule are tracked in DW-022.

### Story close-out

When a story keeps a domain rule out of UI tests, check a unit test covers it and add one if not. Before marking a story done, record its build and UI test runs in the spec memlog with per-project counts, and add any production change missing from its Tasks.

UI tests prove button/action wiring, navigation and displayed information. Domain calculations, copying/identity semantics and exhaustive command-history permutations belong in normal unit tests. Keep variants for distinct UI controls or visible states, not every domain input combination. Synchronization helpers wait for one relevant render marker; they must not silently add unrelated assertions or ordering requirements.

### Directional coverage

Use real application services, with no fakes or DI replacements. Separate the two directions for every service-backed capability:

- **UI to service:** drive the typed control, then assert the observable effect on `ICharacterRepository`, `IFightContext`, or `IUndoableMediator` (including history/redo-history deltas for Undo/Redo). Assert the intended entity/value or command, not an unrelated render result. Keep distinct player/monster and Add/Duplicate controls covered.
- **Service to UI:** seed values independently before navigation, or change the real service after rendering through its normal notification/command path, then assert typed UI output. Do not click the action being tested to manufacture the rendering state. Cover live subscriptions where supported; repository-backed editors are opened from seeded data, not assumed to subscribe to repository changes.
- **UI-local exceptions:** search filters and manual dashboard selection have no service owner. Test those local interactions through the UI and name the exception; do not introduce a production selection service, hooks, or fake just for testing. Turn-service-driven highlighting has its own rendering test.
- **Circuit-scoped drafts:** attack changes are not visible in the shared repository until the parent character is saved. Complete that real Save before asserting stored attacks. Resolving `IGlobalEditContext` or `IAttackEditContext` from root `Services` would inspect a different scope. Rendering tests seed attacks independently and navigate to the real editor.

Action helpers must not wait for roster/list/turn rendering as their completion signal. Use real-service events (subscribe before action, bounded wait, unsubscribe in `finally`), bounded service-state assertions, or necessary navigation/modal completion. Rendering tests use typed retrying expectations for their own markers. These are focused real-browser integration tests, not isolated unit tests. Additional end-to-end sequences may deliberately cross both directions when the interaction is the subject.

Assert with FluentAssertions only, not NUnit constraints such as `Assert.That(...).After(...)`. Await an asynchronous service effect with `Eventually` (`uitests/Framework/Extensions/EventuallyExtensions.cs`), which retries a FluentAssertions chain against the real service. Resolve repeated services and page objects once in `[SetUp]` fields of the fixture that uses them rather than in each test. Mediator completion waits are safe: `OnCommandExecuted` fires only for top-level `SendAsync` after its sub-commands finish, and undo/redo events fire once after the cascade, provided handlers dispatch children with `SendAsSubCommandAsync`.

`WaitForAvailable(groupName, count)` shares row-count synchronization across Players and Monsters without asserting names or order. `WaitForRound` waits on one round/idle marker; do not immediately repeat the same assertion after waiting. Assertions about related visible information, such as another row remaining displayed or Redo becoming enabled after Undo, are legitimate UI checks.

Choose the base by test purpose, not by the number of clicks:

| Sub-case | Base | Behaviour |
|---|---|---|
| Dependent steps (e.g. `CheckEveryStepAlongTheWay`) | `SequentialScenarioFixture` | `[Order(n)]` on every step (else NUnit runs them alphabetically). State is reset only before the first and after the last step; a failed step skips the rest. The fixture runs as a whole. |
| Self-contained tests (e.g. `UndoButtonWorksInAnyCase`) | `IsolatedScenarioFixture` | State reset after each test; one failure does not affect the others. No teardown opt-out, undo/redo tests included; a test asserting initial Redo availability must account for the stale redo stack (DW-023). |
| One behavior requiring several interactions (e.g. edit an attack, save its parent, check the repository) | `IsolatedScenarioFixture` | Still one independent case; navigation and setup do not make it a chained scenario. |

Use `SequentialScenarioFixture` only when interaction between successive business actions is the subject, such as a complete combat scenario. Do not turn CRUD or search/add/remove bundles into ordered chains merely because they contain several actions. Arrangement may use existing domain services; assertions follow the direction under test above. All browser interactions and reads use typed objects, but service assertions belong directly in tests. Related assertions may stay together when they prove one behavior, including preservation of unrelated entities.

The unit of isolation is the fixture (CAP-10). A step of a sequential fixture run alone is expected to fail.

## Namespaces

- `RootNamespace` and every file namespace mirror the location: `DnDFightTool.UiTests.{path below uitests/}`.
  - `uitests/Framework/Pages/` → `DnDFightTool.UiTests.Framework.Pages`
  - `uitests/TestableComponents/UI/TestableFightBlazorComponents/Log/` → `DnDFightTool.UiTests.TestableComponents.UI.TestableFightBlazorComponents.Log`
  - `uitests/Tests/Components/DndUi.SharedUiTests/Components/Pages/` → `DnDFightTool.UiTests.Tests.Components.DndUi.SharedUiTests.Components.Pages`
- `AssemblyFixture` stays in `DnDFightTool.UiTests` — NUnit applies a `[SetUpFixture]` only to its namespace and below.

## References

- **Testable projects** reference `Framework`, `Microsoft.Playwright`, and the sibling Testable projects whose objects they return — never their `src/` project. Current graph: `TestableDndUi.Shared` → `TestableCharacterSheetBlazorComponents`, `TestableFightBlazorComponents`; `TestableFightBlazorComponents` → `TestableCharacterSheetBlazorComponents`, `TestableDnDQueryPrompter` (inverse of `src/`, acyclic).
- **Test projects** (and Scratch) reference `Framework`, the Testable projects they drive, `tests/Domain/DomainTestsUtilities`, and `Microsoft.Playwright` directly so the driver lands in their output. Scratch references every Testable project.
- Test projects and Scratch link `Framework/AssemblyFixture.cs` and `Framework/AssemblyInfo.cs` via `<Compile Include=… Link=…>` so the host lifecycle and the no-parallelism rule apply in their own assembly.
- Every project is listed in `DnDFightTool.slnx` (solution folders mirror paths) and `DnDFightTool.CrossPlatform.slnf`.

## Artifacts

Screenshots land under the running test project's own `artifacts/` folder (gitignored), then the test class namespace below the project's assembly name, then the container and nested class, then the test name: `{project}/artifacts/{component path}/{Container}/{NestedFixture}/{TestName}/NN-{step}.png`.

## Current map

The four business test files use independent capability fixtures. None currently needs a sequential fixture; story 6 retains the full combat chain. Framework tests are outside this business-test review. Paths below are relative to `uitests/`.

Story 5b added exhaustive character-editor field coverage: `CharacterMainInfoEditorUiTests`, `AbilityScoresEditorComponentUiTests`, `SkillsEditorComponentUiTests`, and `ResistancesEditorComponentUiTests`, all under `Tests/UI/CharacterSheetBlazorComponentsUiTests/`.

### Reviewed business coverage

| Surface | Required UI evidence |
|---|---|
| Character library | Separate player/monster creation, duplication, save-edit and deletion controls assert repository state; cancellation leaves it unchanged. Independently seeded player/monster list and editor rendering. Duplication proves an additional stored entry, not identity or deep-copy internals. |
| Attack list | Creation, duplication, save-edit, deletion and separate Add/Duplicate/edit cancellation assert stored attacks after parent Save. Separate seeded list/editor rendering; preserve unrelated stored entries where relevant. |
| Fighter search | Separate player and monster partial-name and no-match tests; filtering one list leaves the other unchanged. Search casing permutations and clear-search cases are not required by this review. |
| Fighter addition | Player, first-monster and repeated-monster Add controls assert service membership. Separate notified service additions render In Fight, hide an added player from Available while others remain, and retain monster templates. |
| Monster grouping | Real service additions of the same kind share one counted row; another kind stays separate. Verify rendered grouping, not clone or initiative-inheritance internals. |
| Fighter removal | Player, last-monster and group-subtraction controls assert service membership. Separate notified service removals check available players, retained monster templates and reduced group counts. |
| Dashboard | Seeded name/HP/initiative display; UI-local manual selection; turn-service highlighting. Start/Next assert command dispatch, Undo/Redo assert real history deltas. Separate service-driven start/advance/undo/redo tests check displayed state and relevant button availability, not exhaustive algorithms. |

First additions now use `TestableInitiativeRollQueryHandlerModal` in `TestableDnDQueryPrompter/FightQueries`, whose `RollAndConfirm` drives the existing prompt. `AddPlayer` completes that prompt; `AddMonsterWithInitiative` handles a first monster and `AddMonster` a repeated kind. This modal is distinct from the dashboard's `TestableInitiativeInputDialog`. Story 6 must extend/reuse these objects for deterministic roll input and remaining dialogs, not create a second first-add helper. Tests that do not assert numerical initiative may roll normally; CAP-4 still requires fixed rolls when outcomes depend on them.

| Character editor deep fields | Character basic-info deep fields (Max HP, HP, Armor Class, shield toggle + value, effective AC read), `AbilityScoresEditor` (score, save bonus, mastery bonus), `SkillsEditor` (mastery increase/decrease, governing-ability menu) and `ResistancesEditor` (affinity increase/decrease) assert stored character state via real UI-to-repository action tests and independently seeded repository-to-UI rendering tests. |

Deep attack editor fields remain story 5c. Story 5's bundled matrix is historical and is superseded for current test organization by the rules above.

| `src/` component | Typed object | Test |
|---|---|---|
| `Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor` | `TestableComponents/Components/TestableDndUi.Shared/Components/Pages/TestableCharacterListEditorPage.cs` | `Tests/Components/DndUi.SharedUiTests/Components/Pages/CharacterListEditorPageUiTests.cs` |
| `Components/DndUi.Shared/Components/Pages/FightDashboardPage.razor` | `…/TestableDndUi.Shared/Components/Pages/TestableFightDashboardPage.cs` | `Tests/Components/DndUi.SharedUiTests/Components/Pages/FightDashboardPageUiTests.cs` |
| `Components/DndUi.Shared/Components/Pages/FightersPage.razor` | `…/TestableDndUi.Shared/Components/Pages/TestableFightersPage.cs` | `Tests/Components/DndUi.SharedUiTests/Components/Pages/FightersPageUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/Characters/Pages/CharacterEditorPage.razor` | `TestableComponents/UI/TestableCharacterSheetBlazorComponents/Characters/Pages/TestableCharacterEditorPage.cs` | — |
| `UI/CharacterSheetBlazorComponents/Characters/Components/CharacterMainInfoEditor.razor` | `…/TestableCharacterSheetBlazorComponents/Characters/Components/TestableCharacterMainInfoEditor.cs` | `Tests/UI/CharacterSheetBlazorComponentsUiTests/Characters/Components/CharacterMainInfoEditorUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/AbilityScores/AbilityScoresEditorComponent.razor` | `…/TestableCharacterSheetBlazorComponents/AbilityScores/TestableAbilityScoresEditorComponent.cs` | `Tests/UI/CharacterSheetBlazorComponentsUiTests/AbilityScores/AbilityScoresEditorComponentUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/AbilityScores/AbilityScoreEditorComponent.razor` | `…/TestableCharacterSheetBlazorComponents/AbilityScores/TestableAbilityScoreEditorComponent.cs` | Exercised by `AbilityScoresEditorComponentUiTests` |
| `UI/CharacterSheetBlazorComponents/AbilityScores/AbilityChip.razor` | — | Exercised by `SkillsEditorComponentUiTests` |
| `UI/CharacterSheetBlazorComponents/DamageAffinities/Components/ResistancesEditorComponent.razor` | `…/TestableCharacterSheetBlazorComponents/DamageAffinities/Components/TestableResistancesEditorComponent.cs` | `Tests/UI/CharacterSheetBlazorComponentsUiTests/DamageAffinities/Components/ResistancesEditorComponentUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/Skills/Components/SkillsEditorComponent.razor` | `…/TestableCharacterSheetBlazorComponents/Skills/Components/TestableSkillsEditorComponent.cs` | `Tests/UI/CharacterSheetBlazorComponentsUiTests/Skills/Components/SkillsEditorComponentUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/MartialAttacks/Pages/AttackEditorPage.razor` | `…/TestableCharacterSheetBlazorComponents/MartialAttacks/Pages/TestableAttackEditorPage.cs` | — |
| `UI/CharacterSheetBlazorComponents/MartialAttacks/Components/AttackListEditor.razor` | `…/TestableCharacterSheetBlazorComponents/MartialAttacks/Components/TestableAttackListEditor.cs` | `Tests/UI/CharacterSheetBlazorComponentsUiTests/MartialAttacks/Components/AttackListEditorUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/MartialAttacks/Components/AttackMainInfoEditor.razor` | `…/TestableCharacterSheetBlazorComponents/MartialAttacks/Components/TestableAttackMainInfoEditor.cs` | — |
| `UI/FightBlazorComponents/Entities/FightingCharacters/Components/FighterTile.razor` | `TestableComponents/UI/TestableFightBlazorComponents/Entities/FightingCharacters/Components/TestableFighterTile.cs` | — |
| `UI/FightBlazorComponents/Entities/FightingCharacters/Dialog/InitiativeInputDialog.razor` | `…/TestableFightBlazorComponents/Entities/FightingCharacters/Dialog/TestableInitiativeInputDialog.cs` | — |
| `UI/FightBlazorComponents/Entities/MartialAttacks/MartialAttackSelector.razor` | `…/TestableFightBlazorComponents/Entities/MartialAttacks/TestableMartialAttackSelector.cs` | — |
| `UI/FightBlazorComponents/CombatStatus/CombatStatusPanel.razor` | `…/TestableFightBlazorComponents/CombatStatus/TestableCombatStatusPanel.cs` | — |
| `UI/FightBlazorComponents/Log/CombatLogPanel.razor` | `…/TestableFightBlazorComponents/Log/TestableCombatLogPanel.cs` | — |
| `UI/DnDQueryPrompter/MartialAttackQueries/MartialAttackRollResultQueryHandlerModal.razor` | `TestableComponents/UI/TestableDnDQueryPrompter/MartialAttackQueries/TestableMartialAttackRollResultQueryHandlerModal.cs` | — |
| `UI/DnDQueryPrompter/FightQueries/InitiativeRollQueryHandlerModal.razor` | `TestableComponents/UI/TestableDnDQueryPrompter/FightQueries/TestableInitiativeRollQueryHandlerModal.cs` | Exercised by `FightersPageUiTests` |
