---
title: 'Page objects for the five routable pages'
type: 'feature'
created: '2026-09-17'
status: 'done'
baseline_commit: '63e727faa3747aac25101f0b61f8d40117c08a0a'
review_loop_iteration: 0
context:
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\project-context.md'
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\specs\spec-005-ui-test-navigation-library\SPEC.md'
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\specs\spec-005-ui-test-navigation-library\deliverables.md'
---

<frozen-after-approval reason="human-owned intent — do not modify unless human renegotiates">

## Intent

**Problem:** The harness (stories 1–4) drives a real browser, isolates scenarios and captures screenshots, but a scenario can only reach the UI through raw Playwright locators. CAP-2 requires the whole application to be operable through typed fluent objects with **zero locator expressions in a scenario body** — and not just each page's "primary action": *every* navigational and CRUD thing a page can do (switch tabs, create/edit/duplicate/delete, search, add to fight, start combat, select a fighter, trigger an attack) must be a method, so an author can walk the entire app in code.

**Approach:** Model two kinds of typed objects. A **`PageObject`** for each routable page (`CharacterListEditorPage`, `FightDashboardPage`, `FightersPage`, `CharacterEditorPage`, `AttackEditorPage`), navigable by route where the URL is enough and returned from a parent's action where it is not. A **`ComponentObject`** for each reusable fragment these pages host (attack-list editor, fighter tile, combat-log panel, combat-status panel, attack selector, plus the character/attack main-info editors at name level). A page method that opens a modal returns a **dialog seam** — a typed placeholder introduced here and implemented in story 6. This story delivers full page-and-navigation coverage; the exhaustive *field-level* editing of the character and attack editors (abilities, skills, resistances, damage rolls, HP/AC/shield) is deferred to a follow-up story. Every non-modal capability in scope gets one `IsolatedScenarioFixture` scenario proving it works with no locator in the scenario body.

## Boundaries & Constraints

**Always:** Typed objects live under `uitests/UiTestNavigation/Pages/` (routable `PageObject`s) and `uitests/UiTestNavigation/Components/` (`ComponentObject`s), namespaced `DnDFightTool.UiTests.UiTestNavigation.{Pages|Components}`. Every locator lives inside a page/component object — never in a scenario, a flow, or a test body. Objects take the Playwright `IPage` (and, for a component, its root `ILocator` scope) through their constructor; page objects also take the host base URL. A page/component object exposes a method for every action its element supports **within this story's scope**, named for the user intent (`GoToMonstersTab`, `CreatePlayer`, `AddToFight`, `StartCombat`, `SelectFighter`, `Edit`, `Duplicate`, `Delete`), plus read methods for asserting (`PlayerNames`, `InFightNames`, `LogEntries`, `GetRound`). A method that navigates returns the destination page object; a method that opens a modal returns its dialog seam. List/dashboard/fighters pages navigate by route (`/`, `/fight-dashboard`, `/fighters`); the editor pages are returned from the parent list page's create/edit/duplicate methods because their subject comes from a per-circuit scoped context the test process cannot seed. Locators use Playwright role and text/label semantics; rows are found by their name text and the action button by role + name within that row. Fight arrangement seeds a non-zero initiative (`AddToFightAtomicCommand(id, initiative: 15)`) so the dashboard's initiative dialog never opens. Follow the repo C# rules: file-scoped namespaces, no primary constructors, no expression-bodied members, XML docs on public/internal members, NUnit + FluentAssertions.

**Ask First:** Any accessible-name (`aria-label`) addition beyond the atomic button presets, or any `data-testid` on production markup — the presets are the only production change this story sanctions.

**Never:** No locator expression, URL, or `GotoAsync` in a scenario body. No scenario that can only be *fully* proven once story 6's dialogs exist — the modal-trigger methods and their seams ship without a scenario here. No exhaustive editor field coverage (abilities, skills, resistances, damage rolls, HP/AC/shield) — that is the deferred follow-up story; here the editors expose only name-level editing needed to create/save an entity. No flow/arrangement-helper layer. Do not implement the dialog seams' interaction surface (story 6). Do not make the editor pages URL-navigable by seeding the scoped context from the test process. Do not touch `MauiProgram.cs` or production hosting.

## I/O & Edge-Case Matrix

| Scenario | Input / State | Expected Output / Behavior | Error Handling |
|----------|--------------|---------------------------|----------------|
| CREATE_PLAYER_VIA_UI | Empty repository, Players tab | `CreatePlayer()` → editor; `MainInfo().SetName(...)`; `Save()`; the player appears in the Players tab | — |
| CREATE_MONSTER_VIA_UI | Empty repository | `GoToMonstersTab().CreateMonster()` → editor; set name; `Save()`; the monster appears in the Monsters tab | — |
| DUPLICATE_DELETE_CHAR | Two players seeded via repository | `DuplicatePlayer(name)` → editor → `Save()` adds a copy; `DeletePlayer(name)` removes it from the list | — |
| ATTACK_CRUD | Character in editor, Attacks tab | `OpenAttacks().AddAttack()` → `AttackEditorPage`; `MainInfo().SetName(...)`; `Save()`; the attack is listed; `EditAttack`/`DuplicateAttack`/`DeleteAttack` behave | — |
| FIGHTERS_ADD_SEARCH | One player + one monster seeded | `SearchMonsters(text)` filters; `AddMonster(name)` moves it to "In Fight"; `RemoveFromFight(name)` takes it back | — |
| DASHBOARD_READ_AND_COMBAT | Two fighters seeded with initiative 15 | Dashboard renders both tiles (name/HP/initiative readable); `CombatStatus().StartCombat()` shows "Round 1"; `SelectFighter(name)` marks it; `Undo()`/`Redo()` toggle | — |
| ATTACK_TRIGGER_SEAM | Fight with a fighter owning an attack | `Attacks().SelectAttack(name).Attack()` returns the martial-attack roll **dialog seam**; no scenario drives the modal (story 6) | — |

</frozen-after-approval>

## Code Map

**Routable pages (targets of `PageObject`s)**
- `src/Components/DndUi.Shared/Components/Pages/CharacterListEditorPage.razor(.cs)` -- `/`, `/Characters`; `MudTabPanel` "Players"/"Monsters"; each `MudPaper` row shows `@Name` with `Edit`/`Duplicate`/`Delete` (Size.Small icon buttons); per-tab bottom `AddNew(type)` large button.
- `src/Components/DndUi.Shared/Components/Pages/FightDashboardPage.razor(.cs)` -- `/fight-dashboard`; `FighterTile` per fighter; `MudTabPanel` "Attacks" hosts `MartialAttackSelector`; `CombatLogPanel`; `CombatStatusPanel`. `CheckForFightersWithoutInitiative` opens `InitiativeInputDialog` when any `InitiativeRoll == 0`.
- `src/Components/DndUi.Shared/Components/Pages/FightersPage.razor(.cs)` -- `/fighters`; "Available" (Players/Monsters, each a search `MudTextField` + per-row `AddButton`) and "In Fight" (`DeleteButton`/`SubtractButton` per entry); `AddToFightCommand`/`RemoveFromFightCommand`.
- `src/UI/CharacterSheetBlazorComponents/Characters/Pages/CharacterEditorPage.razor(.cs)` -- `/Characters/Edit`; renders only when `GlobalEditContext.Character` set → not URL-reachable. Tabs: "Basic infos", "Abilities & Skills", "Resistances", "Attacks"; each tab has `Cancel`/`Save`. This story wires the "Basic infos" (name) and "Attacks" tabs; the other two tabs' deep editing is the deferred story.
- `src/UI/CharacterSheetBlazorComponents/MartialAttacks/Pages/AttackEditorPage.razor(.cs)` -- `/Attacks/Edit`; renders only when `AttackEditContext.Attack` set → not URL-reachable; reached from the editor's Attacks tab.

**Fragments in scope (targets of `ComponentObject`s)**
- `.../Characters/Components/CharacterMainInfoEditor.razor:19` -- `MudTextField Label="Name"`; name-level only here (HP/AC/shield fields → deferred story).
- `.../MartialAttacks/Components/AttackListEditor.razor` -- `AddButton` (`AddNew`) + per-row `@attack.Name` with `Edit`/`Duplicate`/`Delete`.
- `.../MartialAttacks/Components/AttackMainInfoEditor.razor` -- `MudTextField Label="Name"`; name-level only (to-hit modifiers → deferred story).
- `src/UI/FightBlazorComponents/Entities/FightingCharacters/Components/FighterTile.razor` -- `MudCard id="fighter-{Id}"`, `@onclick` selects (only when combat started), `@Fighter.Name` (h4), initiative, HP, health bar, status chips, `Edit`/`Delete`.
- `src/UI/FightBlazorComponents/CombatStatus/CombatStatusPanel.razor(.cs)` -- `UndoButton`/`RedoButton` (Size.Small); "Round N" + turn text when started; primary `MudButton` label `Start Combat`→`Next Turn` (`StartNextTurnCommand`, no modal); disabled when no fighters.
- `src/UI/FightBlazorComponents/Entities/MartialAttacks/MartialAttackSelector.razor` -- `MudTable` of the selected fighter's attacks (row click selects); `FightButton Label="Attack"` → `AttackAsync` opens the roll modals (dialog seam / story 6).
- `src/UI/FightBlazorComponents/Log/CombatLogPanel.razor` -- the combat log; read entry text for assertions.

**Production markup (accessible names)**
- `src/UI/SharedComponents/Buttons/AtomicButtonsPreset/ButtonBase.razor(.cs)` -- `MudIconButton`/`MudButton` render icon-only with no accessible name. Add an `AccessibleName` rendered as `aria-label` on both branches.
- `.../AtomicButtonsPreset/{Edit,Duplicate,Delete,Add,Save,Cancel,Subtract,Undo,Redo,Fight}Button.cs` -- supply each preset's `AccessibleName`.

**Harness (read-only reuse)**
- `uitests/UiTestNavigation/ApplicationFixture.cs` -- protected `Page`, `BaseUrl`, `Services`, `CaptureAsync`; scenarios build page objects from `Page`/`BaseUrl` and arrange via `Services`.
- `uitests/UiTestNavigation/IsolatedScenarioFixture.cs` -- base for the new per-capability scenarios.
- `uitests/UiTestNavigation/Meta/ScenarioIsolationTests.cs` -- reference for arranging via `ICharacterRepository`/`IUndoableMediator` + `CharacterFactory.BuildMonster/BuildPlayer`.

## Tasks & Acceptance

**Execution — production markup:**
- [x] `src/UI/SharedComponents/Buttons/AtomicButtonsPreset/ButtonBase.razor` + `.razor.cs` -- add `AccessibleName`, render as `aria-label` on the Large `MudButton` and the Small/Medium `MudIconButton` -- lets page objects locate icon-only buttons by role + name.
- [x] `.../AtomicButtonsPreset/{Edit,Duplicate,Delete,Add,Save,Cancel,Subtract,Undo,Redo,Fight}Button.cs` -- set each `AccessibleName`.

**Execution — typed-object infrastructure:**
- [x] `uitests/UiTestNavigation/Pages/PageObject.cs` -- NEW abstract base: `IPage Page`, host base URL, `GotoAsync(route)`.
- [x] `uitests/UiTestNavigation/Components/ComponentObject.cs` -- NEW abstract base: `IPage Page` + a root `ILocator` scope so a fragment's locators resolve within its element.
- [x] `uitests/UiTestNavigation/Dialogs/DialogSeam.cs` (+ `MartialAttackRollDialog`, `InitiativeDialog` seams) -- NEW minimal placeholder objects returned by modal-trigger methods; documented as completed in story 6. No interaction methods here.

**Execution — page objects:**
- [x] `uitests/UiTestNavigation/Pages/TestCharacterListEditorPage.cs` -- `GoToAsync`, `GoToPlayersTab`/`GoToMonstersTab`, `CreatePlayer`/`CreateMonster` → `TestCharacterEditorPage`, `EditPlayer`/`EditMonster`/`DuplicatePlayer`/`DuplicateMonster` → `TestCharacterEditorPage`, `DeletePlayer`/`DeleteMonster`, `PlayerNames`/`MonsterNames`.
- [x] `uitests/UiTestNavigation/Pages/TestCharacterEditorPage.cs` -- `MainInfo()` (→ `TestCharacterMainInfoEditor`), `OpenAttacks()` (→ `TestAttackListEditor`), `Save`, `Cancel`. (Abilities/Skills/Resistances tab objects are the deferred story.)
- [x] `uitests/UiTestNavigation/Pages/TestAttackEditorPage.cs` -- `MainInfo()` (→ `TestAttackMainInfoEditor`), `Save`, `Cancel`.
- [x] `uitests/UiTestNavigation/Pages/TestFightersPage.cs` -- `GoToAsync`, `SearchPlayers`/`SearchMonsters`, `AddPlayer`/`AddMonster`, `RemoveFromFight`, `AvailablePlayerNames`/`AvailableMonsterNames`/`InFightNames`.
- [x] `uitests/UiTestNavigation/Pages/TestFightDashboardPage.cs` -- `GoToAsync`, `FighterTile(name)`, `SelectFighter(name)`, `Log()`, `CombatStatus()`, `Attacks()`; `Attacks().SelectAttack(name).Attack()` returns `MartialAttackRollDialog` seam.

**Execution — component objects** (each named to match its production component 1:1):
- [x] `uitests/UiTestNavigation/Components/TestCharacterMainInfoEditor.cs` -- `SetName`, `GetName` (deep fields deferred).
- [x] `uitests/UiTestNavigation/Components/TestAttackListEditor.cs` -- `AddAttack` → `TestAttackEditorPage`, `EditAttack`/`DuplicateAttack` → `TestAttackEditorPage`, `DeleteAttack`, `AttackNames`.
- [x] `uitests/UiTestNavigation/Components/TestAttackMainInfoEditor.cs` -- `SetName`, `GetName` (to-hit modifiers deferred).
- [x] `uitests/UiTestNavigation/Components/TestFighterTile.cs` -- `GetName`, `GetCurrentHp`/`GetMaxHp`, `GetInitiative`, `Statuses`, `Edit` → `TestCharacterEditorPage`, `Delete`.
- [x] `uitests/UiTestNavigation/Components/TestCombatStatusPanel.cs` -- `StartCombat`/`NextTurn`, `Undo`, `Redo`, `GetRound`, `GetTurnText`, `IsCombatStarted`.
- [x] `uitests/UiTestNavigation/Components/TestCombatLogPanel.cs` -- `Entries` (visible text).
- [x] `uitests/UiTestNavigation/Components/TestMartialAttackSelector.cs` -- `SelectAttack(name)`, `Attack()` → `MartialAttackRollDialog` seam.

**Execution — scenarios:**
- [x] `uitests/UiTestNavigation/Scenarios/**` -- NEW `IsolatedScenarioFixture` scenarios covering each non-modal I/O-matrix row, capturing named screenshots and asserting only through typed objects. No scenario for the modal-trigger seams (story 6).

**Acceptance Criteria:**
- Given the character list, when a scenario creates a player through the UI (set name, save) and re-reads the list, then the player is listed and the scenario contains no Playwright locator expression.
- Given a character in the editor, when the scenario adds, edits, duplicates and deletes attacks through `AttackEditorPage`, then the attack list reflects each operation.
- Given a seeded player and monster, when the scenario searches, adds to the fight and removes, then the "In Fight" list updates accordingly.
- Given two fighters seeded with a non-zero initiative, when the scenario navigates to the dashboard, then both tiles render with readable name/HP/initiative, `StartCombat` shows "Round 1", `SelectFighter` marks a tile, and `Undo`/`Redo` toggle — with no initiative dialog opening.
- Given the attack selector, when the scenario calls `Attack()`, then it receives a `MartialAttackRollDialog` seam (compiles, returns typed), and no scenario attempts to drive the modal.
- Given the full `uitests/UiTestNavigation` suite, when it runs in randomised order, then every scenario passes.

## Design Notes

**PageObject vs ComponentObject.** A `PageObject` owns a route (or is returned from a parent action) and composes `ComponentObject`s; a `ComponentObject` is constructed with a root `ILocator` so its actions resolve inside that fragment even when the same component appears twice (e.g. two Save/Cancel rows across tabs, or two search boxes). This is what lets the surface grow without locator collisions, and it is the seam the deferred field-coverage story extends.

**Editor pages are flow-reachable, not URL-reachable.** They render only when `IGlobalEditContext.Character` / `IAttackEditContext.Attack` is set — per-circuit scoped services the test process cannot seed. So their page objects are the return value of the parent's create/edit/duplicate methods, which click the production control that drives the real navigation.

**Dialog seams keep the page surface complete without pre-empting story 6.** `MartialAttackSelector.Attack()` returns a minimal seam type now; story 6 gives the seam its interaction methods and the scenarios that drive it. Story 5 ships the trigger + seam but no modal scenario.

**Initiative modal is side-stepped, not driven.** Seeding fighters with `AddToFightAtomicCommand(id, initiative: 15)` leaves nothing for `CheckForFightersWithoutInitiative` to prompt, so the dashboard renders directly. Driving that dialog through the UI is story 6.

**Accessible name is the sanctioned production change.** Icon-only atomic presets have no accessible name; adding `AccessibleName`→`aria-label` (a real accessibility improvement) keeps locators on the role + name semantics the SPEC mandates instead of a `data-testid` campaign.

## Verification

**Commands:**
- `dotnet build DnDFightTool.slnx` -- expected: SUCCESS.
- `dotnet test uitests/UiTestNavigation/UiTestNavigation.csproj` -- expected: the new per-capability scenarios pass alongside the existing Meta scenarios.

**Manual checks:**
- After the run, `uitests/UiTestNavigation/artifacts/` holds each new scenario's step PNGs ending in `*-final.png`.
- Grep the `Scenarios/` folder: no `GetByRole`/`Locator`/`GotoAsync`/URL string appears in any scenario body.

## Suggested Review Order

**The one production change — accessible names**

- The design keystone: icon-only buttons render `aria-label`, so page objects locate them by role + name instead of a `data-testid` campaign.
  [`ButtonBase.razor:13`](../../../../src/UI/SharedComponents/Buttons/AtomicButtonsPreset/ButtonBase.razor#L13)

**Typed-object architecture**

- Routable base: navigate by route; every locator is owned here, never in a scenario.
  [`PageObject.cs:49`](../../../../uitests/UiTestNavigation/Pages/PageObject.cs#L49)

- Fragment base scoped to a root `ILocator` — the seam that lets the surface grow without locator collisions.
  [`ComponentObject.cs:12`](../../../../uitests/UiTestNavigation/Components/ComponentObject.cs#L12)

- Minimal typed placeholder for a modal, so the trigger is complete now and story 6 fills the interaction.
  [`DialogSeam.cs:11`](../../../../uitests/UiTestNavigation/Dialogs/DialogSeam.cs#L11)

**Navigation model (the interesting decisions)**

- Editors are flow-reachable, not URL-reachable: `CreatePlayer` returns the editor by clicking the real control.
  [`TestCharacterListEditorPage.cs:80`](../../../../uitests/UiTestNavigation/Pages/TestCharacterListEditorPage.cs#L80)

- The editor page is only ever returned from its parent — never navigated to directly.
  [`TestCharacterEditorPage.cs:13`](../../../../uitests/UiTestNavigation/Pages/TestCharacterEditorPage.cs#L13)

- `GoToAsync` reaches the dashboard through a real in-app click to seed nav history (works around DW-007).
  [`TestFightDashboardPage.cs:26`](../../../../uitests/UiTestNavigation/Pages/TestFightDashboardPage.cs#L26)

- Selection wait is token-anchored to the `active` class (post-review fix), aligned with `TestFighterTile.IsSelected`.
  [`TestFightDashboardPage.cs:51`](../../../../uitests/UiTestNavigation/Pages/TestFightDashboardPage.cs#L51)

**Modal trigger seam**

- `Attack()` opens the roll modal and returns the seam; no story-5 scenario drives it.
  [`TestMartialAttackSelector.cs:37`](../../../../uitests/UiTestNavigation/Components/TestMartialAttackSelector.cs#L37)

**Proof (tests)**

- Non-modal capabilities driven only through typed objects — create, cancel, duplicate, delete.
  [`CharacterCreationScenarios.cs:20`](../../../../uitests/UiTestNavigation/Scenarios/CharacterCreationScenarios.cs#L20)

