# Deliverables

Three deliverables, strictly ordered. Each is reviewable on its own. Deliverable 3 is
authored only after 1 and 2 are agreed, so the skills it produces encode final decisions
rather than intentions.

---

## Deliverable 1 — The navigation library

The typed page-object library and its fixture, plus scenarios covering the existing UI.
Lives under a new top-level `/uitests` folder, consumed by project reference. Fixtures and
base classes are in `uitests/Framework`, typed objects in per-origin
`uitests/TestableComponents/**/Testable{OriginProject}` projects, tests in per-origin
`uitests/Tests/**/{OriginProject}UiTests` projects — see [uitests-layout.md](uitests-layout.md).

**Contents**

| Piece | What it is |
|---|---|
| `RegisterWebAppServices` refactor | `DndUi.Web`'s inline registrations extracted into an IoC extension taking `dataFolder`, called by both `Program.cs` and the fixture. Production refactor, prerequisite for everything else. |
| Application fixture | Starts `DndUi.Web` on real Kestrel at a dynamic port and manages the Playwright browser lifecycle. One launch per assembly. |
| Isolation | Per-run unique temp folder for the real `LocalFileCharacterRepository`; teardown undoes every command via `UndoLastCommandAsync()` while `HistoryLength > 0`, then deletes remaining characters through the existing repository API. No `Reset()` anywhere. |
| Page objects | One typed fluent `Testable{Page}` object per routable page (`CharacterListEditorPage`, `FightDashboardPage`, `FightersPage`, `CharacterEditorPage`, `AttackEditorPage`), in its origin's `Testable{OriginProject}` project at the page's path. |
| Dialog objects | One `Testable{Dialog}` object per modal — the `DnDQueryPrompter` modals (initiative roll, martial-attack roll result, martial-attack interaction request, save roll result), `InitiativeInputDialog`, and the `RollableDialogBase` surface — each at its dialog's path. |
| Roll input helpers | Typed access to the numeric roll fields inside the query modals, so a scenario pins outcomes by typing the value (CAP-4). |
| Shared arrangement | Common starting state in per-test `[SetUp]`, optionally in a local inherited fixture; parameterized seed helpers only where inputs differ. Real services arrange action tests and independently drive rendering tests. No pre-emptive flow layer (CAP-5). |
| Screenshot capture | Named-capture API plus an automatic final-state capture per scenario, written to a gitignored artifacts folder (CAP-6). |
| Scenarios | Committed NUnit tests proving UI actions, navigation and displayed information. Each lives in its origin's `uitests/Tests/**/{OriginProject}UiTests` project at the component's path, in `{TestedComponent}UiTests.cs` with nested capability fixtures. Separate player/monster and Add/Duplicate controls have explicit tests; sequential fixtures are reserved for interacting business scenarios. Framework self-tests remain separate in `uitests/Tests/Framework/UiTestFrameworkUiTests/Meta/`. |
| Scratch project | Committed shell at `uitests/Scratch` (csproj + README) whose scenarios are gitignored, where agents write throwaway verification scenarios against every Testable project (CAP-7). |

**Covers** CAP-1 through CAP-6, and CAP-10.

The reviewed business coverage and synchronization rules live in [uitests-layout.md](uitests-layout.md). First-add initiative interaction now supports player availability, monster-template retention and grouping tests; story 6 still owns deterministic roll input and the full combat/dialog scenario. These follow-up edits are not a claim that deliverable 1 is complete or that the latest tests pass.

Every service-backed capability separates typed UI action -> real-service assertion from independent real-service state -> typed rendering assertion. No fakes or service replacements. Pure component-local search/selection remains UI-local; browser-scoped attack drafts require parent Save before repository observation. Action helpers do not depend on rendered business results. Real-browser tests remain integration tests; the full combat chain supplements, rather than replaces, these directional cases. This applies to future editor-field and dialog coverage too.

**Done when** every routable page and dialog surface is reachable through a typed object,
the committed fixtures pass in any fixture order, and a test file contains no
Playwright locator expression.

---

## Deliverable 2 — The page-object coverage analyzer

A Roslyn analyzer in the existing [DnDFightTool.Analyzers](../../../src/Analyzers/DnDFightTool.Analyzers)
project (netstandard2.0, Roslyn 4.8) that flags any component carrying an `@page`
directive with no corresponding page object.

**The reference-direction problem.** The analyzer must see both the UI project and the
page objects (`uitests/TestableComponents/**/Testable{OriginProject}`), which reference in
one direction only — and Testable projects deliberately never reference `src/`. Two viable
resolutions, to be chosen during implementation:

- a small shared contracts assembly holding a marker attribute such as
  `[CoversPage(typeof(FightDashboardPage))]`, referenced by both sides — this needs a
  `src/` reference from the Testable projects, which the current layout rules out; or
- an `AdditionalFiles` coverage manifest listing which pages are covered, or matching the
  `Testable{Page}` naming convention by file name.

**Deliberately not built:** a source generator emitting selector constants from `.razor`
markup, and an analyzer mandating `data-testid` on interactive components. Both were
considered and rejected — the scenarios from deliverable 1 already fail when an existing
page drifts, so those mechanisms would be redundant weight. The analyzer exists to catch
the one case scenarios cannot: a page that is *new* and therefore has no scenario yet.

**Covers** CAP-8.

**Done when** adding a routable page without a page object fails the build with an error
naming that page, and adding the page object clears it.

---

## Deliverable 3 — Skills, agents, and instructions

Authored last, from the finished shape of 1 and 2.

**Contents**

| Piece | What it is |
|---|---|
| `.razor`-scoped instructions | An update to the existing `.github/instructions/blazor-components.instructions.md` (or a sibling) stating that a UI change must carry the matching page-object and scenario update. |
| Page-object authoring skill | Conventions, the fluent shape, locator strategy, the `/uitests` placement and naming rules from [uitests-layout.md](uitests-layout.md), and worked examples of both UI-to-real-service and real-service-to-UI tests, including no-fakes, local-state/draft boundaries and direction-specific synchronization. |
| Agent verification workflow | How an agent writes a scratch scenario, runs `dotnet test`, and reports assertions plus screenshot paths as evidence (CAP-7). |

**Covers** CAP-7 and CAP-9.

**Done when** an agent, given only these files, adds a page object for a new page and
produces screenshot evidence for a feature without further guidance.
