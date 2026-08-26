# Deliverables

Three deliverables, strictly ordered. Each is reviewable on its own. Deliverable 3 is
authored only after 1 and 2 are agreed, so the skills it produces encode final decisions
rather than intentions.

---

## Deliverable 1 — The navigation library

The typed page-object library and its fixture, plus scenarios covering the existing UI.
Lives under a new top-level `/uitests` folder, consumed by project reference.

**Contents**

| Piece | What it is |
|---|---|
| `RegisterWebAppServices` refactor | `DndUi.Web`'s inline registrations extracted into an IoC extension taking `dataFolder`, called by both `Program.cs` and the fixture. Production refactor, prerequisite for everything else. |
| Application fixture | Starts `DndUi.Web` on real Kestrel at a dynamic port and manages the Playwright browser lifecycle. One launch per assembly. |
| Isolation | Per-run unique temp folder for the real `LocalFileCharacterRepository`; teardown undoes every command via `UndoLastCommandAsync()` while `HistoryLength > 0`, then deletes remaining characters through the existing repository API. No `Reset()` anywhere. |
| Page objects | One typed fluent object per routable page: `CharacterListEditorPage`, `FightDashboardPage`, `FightersPage`, `CharacterEditorPage`, `AttackEditorPage`. |
| Dialog objects | Typed objects for the `DnDQueryPrompter` modals (initiative roll, martial-attack roll result, martial-attack interaction request, save roll result), `InitiativeInputDialog`, and the `RollableDialogBase` surface. |
| Roll input helpers | Typed access to the numeric roll fields inside the query modals, so a scenario pins outcomes by typing the value (CAP-4). |
| Composable flows | Named building blocks — create a monster, add it to a fight, roll initiative — free to call domain services directly rather than going through page objects (CAP-5). |
| Screenshot capture | Named-capture API plus an automatic final-state capture per scenario, written to a gitignored artifacts folder (CAP-6). |
| Scenarios | Committed NUnit scenarios exercising each page and dialog, which are also the proof the library works. |
| Scratch project | Committed empty shell under `/uitests` whose contents are gitignored, where agents write throwaway verification scenarios (CAP-7). |

**Covers** CAP-1 through CAP-6, and CAP-10.

**Done when** every routable page and dialog surface is reachable through a typed object,
the committed scenarios pass in a randomised order, and a scenario file contains no
Playwright locator expression.

---

## Deliverable 2 — The page-object coverage analyzer

A Roslyn analyzer in the existing [DnDFightTool.Analyzers](../../../src/Analyzers/DnDFightTool.Analyzers)
project (netstandard2.0, Roslyn 4.8) that flags any component carrying an `@page`
directive with no corresponding page object.

**The reference-direction problem.** The analyzer must see both the UI project and the
page-object library, which reference in one direction only. Two viable resolutions, to be
chosen during implementation:

- a small shared contracts assembly holding a marker attribute such as
  `[CoversPage(typeof(FightDashboardPage))]`, referenced by both sides; or
- an `AdditionalFiles` coverage manifest listing which pages are covered.

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
| Page-object authoring skill | Conventions, the fluent shape, locator strategy, and a worked example, sufficient for an agent to add a page object unaided. |
| Agent verification workflow | How an agent writes a scratch scenario, runs `dotnet test`, and reports assertions plus screenshot paths as evidence (CAP-7). |

**Covers** CAP-7 and CAP-9.

**Done when** an agent, given only these files, adds a page object for a new page and
produces screenshot evidence for a feature without further guidance.
