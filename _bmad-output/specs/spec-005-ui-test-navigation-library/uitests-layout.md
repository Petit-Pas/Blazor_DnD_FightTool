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

The file holds an empty `public static class {TestedComponent}UiTests`. Each sub-case is a **nested class** deriving from one of the two fixtures:

| Sub-case | Base | Behaviour |
|---|---|---|
| Dependent steps (e.g. `CheckEveryStepAlongTheWay`) | `SequentialScenarioFixture` | `[Order(n)]` on every step (else NUnit runs them alphabetically). State is reset only before the first and after the last step; a failed step skips the rest. The fixture runs as a whole. |
| Self-contained tests (e.g. `UndoButtonWorksInAnyCase`) | `IsolatedScenarioFixture` | State reset after each test; one failure does not affect the others. Undo/redo-subject tests override `UndoAllCommandsOnTeardown` to `false`. |
| One long self-contained test (e.g. `VerySpecificTest`) | `IsolatedScenarioFixture` | Same base, single `[Test]`. |

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

Component test files use the nested-fixture shape; each existing test sits unchanged in an `IsolatedScenarioFixture` nested class (splitting multi-step tests into `SequentialScenarioFixture` chains is left to the test scope review). The Meta framework tests are exempt: they test the framework, not a component. Paths below are relative to `uitests/`.

| `src/` component | Typed object | Test |
|---|---|---|
| `Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor` | `TestableComponents/Components/TestableDndUi.Shared/Components/Pages/TestableCharacterListEditorPage.cs` | `Tests/Components/DndUi.SharedUiTests/Components/Pages/CharacterListEditorPageUiTests.cs` |
| `Components/DndUi.Shared/Components/Pages/FightDashboardPage.razor` | `…/TestableDndUi.Shared/Components/Pages/TestableFightDashboardPage.cs` | `Tests/Components/DndUi.SharedUiTests/Components/Pages/FightDashboardPageUiTests.cs` |
| `Components/DndUi.Shared/Components/Pages/FightersPage.razor` | `…/TestableDndUi.Shared/Components/Pages/TestableFightersPage.cs` | `Tests/Components/DndUi.SharedUiTests/Components/Pages/FightersPageUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/Characters/Pages/CharacterEditorPage.razor` | `TestableComponents/UI/TestableCharacterSheetBlazorComponents/Characters/Pages/TestableCharacterEditorPage.cs` | — |
| `UI/CharacterSheetBlazorComponents/Characters/Components/CharacterMainInfoEditor.razor` | `…/TestableCharacterSheetBlazorComponents/Characters/Components/TestableCharacterMainInfoEditor.cs` | — |
| `UI/CharacterSheetBlazorComponents/MartialAttacks/Pages/AttackEditorPage.razor` | `…/TestableCharacterSheetBlazorComponents/MartialAttacks/Pages/TestableAttackEditorPage.cs` | — |
| `UI/CharacterSheetBlazorComponents/MartialAttacks/Components/AttackListEditor.razor` | `…/TestableCharacterSheetBlazorComponents/MartialAttacks/Components/TestableAttackListEditor.cs` | `Tests/UI/CharacterSheetBlazorComponentsUiTests/MartialAttacks/Components/AttackListEditorUiTests.cs` |
| `UI/CharacterSheetBlazorComponents/MartialAttacks/Components/AttackMainInfoEditor.razor` | `…/TestableCharacterSheetBlazorComponents/MartialAttacks/Components/TestableAttackMainInfoEditor.cs` | — |
| `UI/FightBlazorComponents/Entities/FightingCharacters/Components/FighterTile.razor` | `TestableComponents/UI/TestableFightBlazorComponents/Entities/FightingCharacters/Components/TestableFighterTile.cs` | — |
| `UI/FightBlazorComponents/Entities/FightingCharacters/Dialog/InitiativeInputDialog.razor` | `…/TestableFightBlazorComponents/Entities/FightingCharacters/Dialog/TestableInitiativeInputDialog.cs` | — |
| `UI/FightBlazorComponents/Entities/MartialAttacks/MartialAttackSelector.razor` | `…/TestableFightBlazorComponents/Entities/MartialAttacks/TestableMartialAttackSelector.cs` | — |
| `UI/FightBlazorComponents/CombatStatus/CombatStatusPanel.razor` | `…/TestableFightBlazorComponents/CombatStatus/TestableCombatStatusPanel.cs` | — |
| `UI/FightBlazorComponents/Log/CombatLogPanel.razor` | `…/TestableFightBlazorComponents/Log/TestableCombatLogPanel.cs` | — |
| `UI/DnDQueryPrompter/MartialAttackQueries/MartialAttackRollResultQueryHandlerModal.razor` | `TestableComponents/UI/TestableDnDQueryPrompter/MartialAttackQueries/TestableMartialAttackRollResultQueryHandlerModal.cs` | — |
