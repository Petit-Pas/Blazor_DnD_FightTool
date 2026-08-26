---
id: SPEC-005-ui-test-navigation-library
companions:
  - deliverables.md
  - ../../project-context.md
sources: []
---

> **Canonical contract.** This SPEC and the files in `companions:` are the complete, preservation-validated contract for what to build, test, and validate. Source documents listed in frontmatter are for traceability — consult them only if you need narrative rationale or prose color this contract intentionally omits.

# UI Test Navigation Library

## Why

A vision to realize, with a pain underneath it. Today the UI of DnDFightTool has no automated coverage at all: [tests/UI/FightBlazorComponentsTests](tests/UI/FightBlazorComponentsTests) is plain NUnit over log-token parsing, and nothing renders a component, clicks a button, or opens a dialog. Every claim that a feature works is a human clicking through the app. That blocks two things at once. Benoit cannot regression-test the fight flow, and an AI agent implementing a feature has no way to *prove* it did so — it can compile, it can run domain tests, but it cannot demonstrate the button it added actually does the thing. A typed navigation library over a real browser closes both: it gives humans repeatable scenarios and gives agents a way to write throwaway verification code and hand back screenshots as evidence. It matters now because agent-driven implementation is already the working mode in this repo, and unverifiable UI work is the weakest link in it.

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
  - **intent:** A scenario reuses the steps proven by earlier scenarios — create a monster, add it to a fight, roll initiative — as named building blocks instead of re-deriving navigation. **A building block is free to bypass the UI and call the domain services directly**; creating a default goblin is arrangement, not the thing under test.
  - **success:** A combat scenario reaches "goblin engaged in an active fight" in a handful of named calls that touch no page object, while a separate scenario proves goblin creation works *through* the UI.

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
  - **success:** The full scenario suite passes in a randomised order, any single scenario passes when run alone, and a scenario whose command undo is incomplete fails in its own teardown rather than corrupting a later scenario.

## Constraints

- Playwright driving the `DndUi.Web` host is the only rendering path. bUnit is excluded: it cannot produce image screenshots, and MudBlazor 9 dialogs and popovers require hand-stubbed JSInterop — precisely the surface CAP-3 targets.
- The library and the agent scratch project live under a new top-level `/uitests` folder, consumed by project reference only. Nothing is packaged.
- The fixture must start real Kestrel on a dynamic port. `WebApplicationFactory`'s default `TestServer` is an in-memory transport with no socket and is unreachable by a browser.
- `DndUi.Web`'s inline service registrations move into `RegisterWebAppServices(services, dataFolder)`, called by both `Program.cs` and the test fixture, matching the existing `IoC/ServiceCollectionExtensions` convention. This is a production refactor so the fixture can compose the same host — not a test hook inside it.
- **No `Reset()` is added to any service.** Cleanup uses two existing mechanisms, split along the seam of what is command-driven:
  - **Command-driven state** (fight, turns, log, applied statuses) is cleared by teardown undoing every command — `UndoLastCommandAsync()` in a loop while `HistoryLength > 0`.
  - **Character persistence is not command-driven** — `ICharacterRepository` is used directly by `CharacterEditorPage`, `CharacterListEditorPage` and `GlobalEditContext`, so undo cannot reach it. Teardown deletes characters through the existing `GetAllCharacters()` and `Delete()` business API.
- Teardown asserts the resulting state is empty. A broken undo therefore fails the scenario that caused it, instead of silently corrupting later ones — the cleanup doubles as continuous proof that undo genuinely restores everything.
- Scenarios whose subject is undo/redo itself opt out of the undo-everything teardown.
- The test host composition sets `CommandHistoryMaxSize` to 1000 (production default is 64), so no realistic scenario can overflow the history and leave state that teardown cannot undo.
- After a full undo the redo stack still holds the undone commands (`RedoHistoryMaxSize` 32). State is clean, but a scenario asserting on undo/redo button availability at its very first step would see a stale redo; the first new command clears the stack.
- Storage is isolated by pointing the real `LocalFileCharacterRepository` at a unique temp folder **per run**. `dataFolder` is already a constructor argument, so no new type is introduced and the real persistence path stays under test. Per-scenario folders are not used — the repository singleton takes `dataFolder` at host-build time, so that would require the rejected per-scenario host rebuild.
- **Arrangement may bypass the UI; the behaviour under test may not.** A building block that puts the app into a starting state is free to call domain services directly. The thing a scenario actually asserts on must be driven through the UI, and every capability reachable through the UI has at least one scenario that exercises it that way.
- Direct service arrangement uses the in-process `IServiceProvider` the fixture already holds. No seeding endpoint, HTTP backdoor, or test-only registration is added to the production host — the fixture composes the same host `Program.cs` does and reaches into it from the same process.
- Only host singletons are reachable this way: `IFightContext`, `ICharacterRepository`, `ICombatTurnService`, `IAppliedStatusRepository`, `IDnDLogService`. Per-circuit scoped services — `IGlobalEditContext`, `IDiceRollNotifier`, `IStateFullNavigation` — belong to the browser's circuit and cannot be arranged from the test process.
- Mutating a singleton after a page has rendered does not repaint it. Arrangement either happens before navigation, or fires the same notification the production path fires (`NotifyFighterUpdated`, `OnFighterAdded`).
- Determinism comes through the UI: `DiceRollResultInputComponent` exposes roll results as an editable numeric field inside the query modals, so a scenario types the exact roll it wants rather than stubbing the dice.
- No blanket `data-testid` campaign across production markup. Locators use Playwright role and text semantics; adding an id is a case-by-case last resort where a locator is genuinely ambiguous.
- Enforcement is the page-object coverage analyzer plus the `.razor`-scoped instructions file. No source generator emitting a selector catalog, and no analyzer mandating `data-testid` — scenarios failing already covers drift on existing pages.
- The analyzer lives in the existing [src/Analyzers/DnDFightTool.Analyzers](src/Analyzers/DnDFightTool.Analyzers) project, netstandard2.0 against Roslyn 4.8, and reports a **build-breaking error**.
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
- Localization-proof selectors.
- Performance, load, or accessibility testing.

## Success signal

An agent finishes implementing a UI feature, writes a scratch scenario against the page objects, runs `dotnet test`, and hands back a passing result plus screenshots showing the feature working — no manual app launch, no hand-written selectors, no human clicking through to confirm. And when someone adds a new page without a page object, they learn it from a build diagnostic that day, not from a stale suite months later.

## Assumptions

- Chromium only; no cross-browser matrix was requested or implied.
- The screenshot output folder is gitignored build output, not committed evidence.
- NUnit parallelization is disabled for the scenario project, following from the host-wide singleton constraint.
- The existing `FightBlazorComponentsTests` project stays as it is; this work adds separate projects under `/uitests`.
- Playwright browser binaries are installed via the standard install step, a documented local prerequisite for running the suite.
- The scratch project is committed as an empty shell whose contents are gitignored.
