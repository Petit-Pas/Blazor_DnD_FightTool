---
id: SPEC-005-ui-test-navigation-library
companions:
  - deliverables.md
  - uitests-layout.md
  - ../../project-context.md
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# UI Test Navigation Library

## Why

DnDFightTool needs repeatable proof that its UI controls invoke the intended actions and display the resulting state. A typed real-browser library lets Benoit regression-test individual capabilities and interacting combat scenarios, while agents can demonstrate changes with assertions and screenshots. This layer verifies the rendered experience; existing unit tests remain responsible for domain correctness.

## Capabilities

- **CAP-1**
  - **intent:** A scenario author starts the application and a browser session by deriving from one fixture, without knowing how either is launched.
  - **success:** A test deriving from the fixture reaches the app's home page and asserts on rendered content, with no port, URL, or browser-launch code anywhere in the test body.

- **CAP-2**
  - **intent:** A scenario navigates to and operates any routable page through a typed fluent object rather than raw selectors.
  - **success:** All five routable pages (`CharacterListEditorPage`, `FightDashboardPage`, `FightersPage`, `CharacterEditorPage`, `AttackEditorPage`) have a page object; a scenario visiting each and performing its primary action compiles and passes with zero Playwright locator expressions written in the test.

- **CAP-3**
  - **intent:** A scenario drives every modal surface, including the roll-result query modals that interrupt combat flow.
  - **success:** A scenario triggering a martial attack completes the hit-roll and damage-roll modals through typed objects and asserts the resulting log entry.

- **CAP-4**
  - **intent:** A scenario pins every random outcome by supplying the roll value through the same modal a human would use.
  - **success:** The same attack scenario run ten times produces identical log output and identical target HP.

- **CAP-5**
  - **intent:** A scenario author reuses common starting-state preparation without duplicating setup or depending on another test.
  - **success:** Independent scenarios share common preparation and retain their scenario-specific starting state. Action tests drive typed UI controls and assert real-service effects; separate rendering tests seed or mutate real services and assert typed UI output. Service-seeded prerequisites retain separate UI-creation coverage.

- **CAP-6**
  - **intent:** A scenario captures a named screenshot at any point, and every scenario captures a final-state image, so a reader can see the feature working.
  - **success:** After a run, a folder holds per-scenario PNGs named by scenario and step, present for passing runs and not only for failures.

- **CAP-7**
  - **intent:** An agent validates a feature it just implemented by writing a throwaway scenario into a dedicated scratch project, running `dotnet test`, and reading assertions plus screenshots.
  - **success:** An agent given a feature description produces a passing scratch scenario and cites the screenshot paths, without editing the library or any committed scenario.

- **CAP-8**
  - **intent:** Whoever adds a new routable page is told at build time that the page has no page object.
  - **success:** Adding a component carrying an `@page` directive with no corresponding page object emits a build diagnostic naming that page; adding the page object clears it.

- **CAP-9**
  - **intent:** An agent modifying UI markup knows, without being reminded, that the corresponding page object and scenario must be updated in the same change.
  - **success:** The `.razor`-scoped instructions file states the obligation, and a skill documents the page-object conventions well enough for an agent to author a new one unaided.

- **CAP-10**
  - **intent:** A scenario starts from a clean slate regardless of what ran before it, without any scenario needing to know what the previous one did, and teardown proves the slate is genuinely clean rather than assuming it.
  - **success:** The unit of isolation is the fixture: every isolated test and every sequential fixture passes in any fixture order and when run alone, a sequential chain runs as a whole, and a fixture whose command undo is incomplete fails in its own teardown rather than corrupting a later one.

## Constraints

- Playwright driving the `DndUi.Web` host is the only rendering path. bUnit is excluded: it cannot produce image screenshots, and MudBlazor 9 dialogs and popovers require hand-stubbed JSInterop — precisely the surface CAP-3 targets.
- The library and the agent scratch project live under a new top-level `/uitests` folder, consumed by project reference only. Nothing is packaged.
- `/uitests` has three kinds of project, each mirroring `src/` exactly: `Framework` (fixtures and typed-object base classes only), `TestableComponents/{layer}/Testable{OriginProject}` (one `Testable{Component}` typed object per component, at the component's path), and `Tests/{layer}/{OriginProject}UiTests` (one `{TestedComponent}UiTests.cs` per tested component — an empty static container whose nested classes derive from `SequentialScenarioFixture` or `IsolatedScenarioFixture`). Placement, namespace and reference rules are in [uitests-layout.md](uitests-layout.md).
- The fixture must start real Kestrel on a dynamic port. `WebApplicationFactory`'s default `TestServer` is an in-memory transport with no socket and is unreachable by a browser.
- `DndUi.Web`'s inline service registrations move into `RegisterWebAppServices(services, dataFolder)`, called by both `Program.cs` and the test fixture, matching the existing `IoC/ServiceCollectionExtensions` convention. This is a production refactor so the fixture can compose the same host — not a test hook inside it.
- **No `Reset()` is added to any service.** Cleanup uses two existing mechanisms, split along the seam of what is command-driven:
  - **Command-driven state** (fight, turns, log, applied statuses) is cleared by teardown undoing every command — `UndoLastCommandAsync()` in a loop while `HistoryLength > 0`.
  - **Character persistence is not command-driven** — `ICharacterRepository` is used directly by `CharacterEditorPage`, `CharacterListEditorPage` and `GlobalEditContext`, so undo cannot reach it. Teardown deletes characters through the existing `GetAllCharacters()` and `Delete()` business API.
- Teardown asserts the resulting state is empty. A broken undo therefore fails the scenario that caused it, instead of silently corrupting later ones — the cleanup doubles as continuous proof that undo genuinely restores everything.
- The test host composition sets `CommandHistoryMaxSize` to 1000 (production default is 64), so no realistic scenario can overflow the history and leave state that teardown cannot undo.
- Every isolated test, undo/redo tests included, runs the undo-everything teardown; there is no opt-out.
- After a full undo the redo stack still holds the undone commands (`RedoHistoryMaxSize` 32). State is clean, but a scenario asserting on undo/redo button availability at its very first step would see a stale redo; the first new command clears the stack. Such a scenario runs a command first or asserts Redo enabled. Clearing the stack is deferred (DW-023).
- Storage is isolated by pointing the real `LocalFileCharacterRepository` at a unique temp folder **per run**. `dataFolder` is already a constructor argument, so no new type is introduced and the real persistence path stays under test. Per-scenario folders are not used — the repository singleton takes `dataFolder` at host-build time, so that would require the rejected per-scenario host rebuild.
- **Separate UI-to-service wiring from service-to-UI rendering.** Action tests drive typed controls and assert the real repository, fight service or mediator history; independent rendering tests seed or mutate real services and read typed UI objects. Every distinct UI control retains action coverage. Do not require UI output to prove an action reached its service, or use that same UI action to arrange its rendering test. These remain real-browser integration tests, not isolated component unit tests. No fakes or service replacements.
- State with no service owner (currently fighter search and manual dashboard selection) remains explicitly UI-local. Do not add production hooks or services to manufacture a boundary. Browser-scoped attack drafts are observed through the real repository after the parent character Save; do not resolve a different scoped context from the host provider. Genuine end-to-end business sequences remain additional integration coverage, not a replacement for directional tests.
- Business fixtures are grouped by capability, with duplication under creation and deletion independent. Individual controls use isolated tests; only genuine interactions across a business scenario justify a sequential fixture. Separate player/monster controls and Add/Duplicate paths use explicitly named tests, not branching type or boolean variants.
- Unconditional common arrangement belongs in per-test setup; scenario-specific data and turn/history state remain explicit. Every test reaches its page the way a user does: a page object's `GoToAsync`, then production controls that return the next typed object. Each nested fixture derives directly from `IsolatedScenarioFixture` or `SequentialScenarioFixture` and owns its `[SetUp]`; no local base fixtures, tools objects or static seeding/navigation helpers. Action synchronization uses service completion/state or navigation/modal completion, not the rendered business result. Rendering synchronization waits for one relevant marker, not an implicit collection of behavioral assertions. Detailed conventions and the reviewed coverage boundary are in [uitests-layout.md](uitests-layout.md).
- Fighters-page coverage must distinguish player removal from Available on addition, monster-template retention, and one counted row per monster kind. First-add initiative interaction supports those checks; deterministic combat outcomes and the remaining dialog coverage remain part of CAP-3/CAP-4.
- Tests seed every starting value they act on or assert against, never relying on factory or constructor defaults. A rendering test of a two-state control checks both states. Test bodies pass typed objects the rendered strings as literals.
- When a story keeps a domain rule out of UI tests, a unit test must cover that rule; the implementer adds one if missing.
- A story is done only after its build and UI test runs are recorded in the memlog with per-project counts. Any production change not in the story's Tasks is added to them before close.
- Direct service arrangement uses the in-process `IServiceProvider` the fixture already holds. No seeding endpoint, HTTP backdoor, or test-only registration is added to the production host — the fixture composes the same host `Program.cs` does and reaches into it from the same process.
- Only host singletons are reachable this way: `IFightContext`, `ICharacterRepository`, `ICombatTurnService`, `IAppliedStatusRepository`, `IDnDLogService`. Per-circuit scoped services — `IGlobalEditContext`, `IDiceRollNotifier`, `IStateFullNavigation` — belong to the browser's circuit and cannot be arranged from the test process.
- Mutating a singleton after a page has rendered does not repaint it. Arrangement either happens before navigation, or fires the same notification the production path fires (`NotifyFighterUpdated`, `OnFighterAdded`).
- Determinism comes through the UI: `DiceRollResultInputComponent` exposes roll results as an editable numeric field inside the query modals, so a scenario types the exact roll it wants rather than stubbing the dice.
- Typed objects locate elements by role + accessible name only: no layout or utility CSS classes, no visible title text as an anchor, no DOM position (`.First`, parent hops). When a control or region lacks a usable accessible name, adding `role`/`aria-label` to production markup is pre-approved and listed in the story's Tasks. `data-testid` stays a case-by-case last resort requiring approval; no blanket campaign.
- Enforcement is the page-object coverage analyzer plus the `.razor`-scoped instructions file. No source generator emitting a selector catalog, and no analyzer mandating `data-testid` — scenarios failing already covers drift on existing pages.
- The analyzer lives in the existing [DnDFightTool.Analyzers](../../../src/Analyzers/DnDFightTool.Analyzers) project, netstandard2.0 against Roslyn 4.8, and reports a **build-breaking error**.
- Scratch scenarios are gitignored, never committed, and wiped between runs. The durable evidence is the screenshots, not the throwaway code.
- Coverage is all five routable pages and all dialog surfaces, not a vertical slice.
- Library and scenario code obeys the C# rules in `project-context.md`: file-scoped namespaces, no primary constructors, no expression-bodied members, XML docs on public and internal members, NUnit with FluentAssertions.
- Scenarios run sequentially. `IFightContext` and its sibling singletons are host-wide, so concurrent scenarios would share fight state.
- Screenshots are evidence artifacts only — no baseline images, no diffing, no failure on visual drift.
- Deliverable 3 is authored only after deliverables 1 and 2 are agreed, so it captures final decisions rather than intentions.

## Non-goals

- Visual regression testing — baseline image capture, diffing, or failing a run on pixel drift.
- bUnit or any in-process component-level rendering tests.
- An MCP server, a CLI scenario runner, or a `dotnet-script` REPL as the agent entry point.
- Automating the MAUI shell. The web host is the target; `DndUi.Shared` is what both hosts share.
- A source-generated selector catalog, or an analyzer enforcing `data-testid` on interactive components.
- A cross-browser matrix. Chromium only.
- CI integration. The project runs locally only — there is no pipeline, no deployment, nothing to hook into.
- Packaging the library for external reuse.
- Replacing existing domain and business unit tests. This layer sits above them, not instead of them.
- UI matrices for domain calculations, clone identities or deep-copy fidelity, persistence/snapshot internals, or exhaustive turn/history rules. A duplicate's provenance alone does not require another deletion test.
- Localization-proof selectors.
- Performance, load, or accessibility testing.

## Success signal

An agent finishes implementing a UI feature, writes a scratch scenario against the page objects, runs `dotnet test`, and hands back a passing result plus screenshots showing the feature working — no manual app launch, no hand-written selectors, no human clicking through to confirm. And when someone adds a new page without a page object, they learn it from a build diagnostic that day, not from a stale suite months later.

## Assumptions

- Chromium only; no cross-browser matrix was requested or implied.
- The screenshot output folder is gitignored build output, not committed evidence.
- NUnit parallelization is disabled in every `/uitests` test project (the shared `AssemblyInfo.cs` is linked into each), following from the host-wide singleton constraint.
- The existing `FightBlazorComponentsTests` project stays as it is; this work adds separate projects under `/uitests`.
- Playwright browser binaries are installed via the standard install step, a documented local prerequisite for running the suite.
- The scratch project is committed as an empty shell whose contents are gitignored.

