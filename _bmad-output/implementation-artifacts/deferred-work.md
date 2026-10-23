# Deferred Work

IDs are stable and never reused. Active entries come first; `resolved` and `dropped`
entries move to the Closed section at the bottom, in ID order. Prune the Closed section
yourself when you no longer want the history.

Status: `open` · `resolved` · `dropped`

---

# Active

## DW-023 — Stale redo stack after the undo-everything teardown

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/SPEC.md`
- summary: `ResetApplicationStateAsync` undoes every command, so the undone commands stay on the redo stack (up to `RedoHistoryMaxSize` 32). The next test starts with Redo enabled until it runs its first command. A test that asserts Redo is disabled on its first step fails, whatever ran before it. The UndoableMediator API cannot clear redo history without running a new command.

Proposed direction (to plan, not decided): add a way to clear redo history in UndoableMediator (library change) and call it at the end of teardown, then assert the redo stack is empty. Until then, a test that asserts initial Redo availability runs a command first or asserts Redo enabled. Note: the earlier teardown opt-out for undo/redo tests was retired on 2026-10-23; it would not fix this.

---

## DW-022 — Migrate typed objects off CSS, title-text and position locators

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/SPEC.md`
- summary: The locator rule now allows role + accessible name only. Older typed objects still use CSS or position, for example `button.w-100:visible` and `.mud-paper:has(.hoverable-button-row)` in `TestableCharacterListEditorPage`, `.combat-status-panel` and `a[href='...']` in the page objects' `GoToAsync`, `.ability-score-chip`, `.mud-menu-item`, `form:visible`.First in `TestableCharacterEditorPage.MainInfo`, and `xpath=ancestor::tr` in `TestableSkillsEditorComponent`.

Proposed direction: audit `uitests/TestableComponents` and `uitests/Framework`, add `role`/`aria-label` to the production markup where a name is missing (pre-approved), and switch each locator to `GetByRole(..., Name = ...)`. Run all UI test projects after each file.

---

## DW-021 — Remove local abstract base fixtures from UI tests

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md`
- summary: Local abstract base fixtures are no longer allowed, because they force every derived fixture onto one base class. `CharacterListEditorPageUiTests.CharacterLibraryFixture`, `FightDashboardPageUiTests.DashboardFixture`, `CharacterMainInfoEditorUiTests.MainInfoEditFixture` and `SkillsEditorComponentUiTests.SkillsEditFixture` still exist.

Proposed direction: each nested fixture derives directly from `IsolatedScenarioFixture` (or `SequentialScenarioFixture`) and resolves its repository, mediator and page object in its own `[SetUp]`. Behaviour must not change; run DndUi.SharedUiTests and CharacterSheetBlazorComponentsUiTests before and after.

---

## DW-020 — Premade test characters, monsters and attacks for reuse across tests

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md`
- summary: UI tests now seed and navigate in plain view, so each test builds its own data inline (`CharacterFactory.BuildPlayer(name: "Fighter", withDefaults: true)`, then sets HP, abilities, skills, affinities or attacks one by one). The same shapes repeat across files: a default "Fighter" player, a player with a Longsword and a Bow, explicit Strength or Fire values. This makes tests long and lets the same idea drift between files.

Proposed direction (to plan, not decided):
- Add named, ready-made test data in `tests/Domain/DomainTestsUtilities` (already referenced by every unit and UI test project), for example a `PremadeCharacters` / `PremadeMonsters` / `PremadeAttacks` set: a plain fighter, an armored fighter with a shield, a goblin, an owlbear, a longsword, a bow.
- Each premade returns a new instance on every call, so tests never share mutable state, and tests can still change fields after building it.
- Keep values a test asserts on explicit in the test (or exposed as constants on the premade), so the test stays readable without opening the factory.
- Keep seeding and navigation in the test or its `[SetUp]` (uitests-layout rule); premades only build objects, they do not save or navigate.
- Decide whether unit tests (DnDActionsTests, FightTests) should use the same premades, and how premades relate to the existing `CharacterFactory`, `MartialAttackTemplateFactory` and friends (extend them or sit on top).

Costs: a shared data catalog couples tests to each other; changing a premade can break unrelated tests unless asserted values stay explicit in each test.

---

## DW-019 — Save-mastery state is read from a MudBlazor CSS class

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5b-exhaustive-character-editor-field-coverage.md`
- summary: `TestableAbilityScoreEditorComponent.HasSaveMastery()` returns true when the ability chip has the `mud-chip-filled` class. That class is internal to MudBlazor, so an upgrade could rename it and silently break the getter. The rendering test checks both true (STR) and false (DEX), so a rename fails loudly, but the coupling stays.

Proposed direction: expose the state on the chip with an accessible attribute, for example `aria-pressed="true|false"` (the chip acts as a toggle), and read that attribute instead. This is a production markup change. Do it together with DW-018, which moves the mastery and affinity state into proper attributes too.

---

## DW-018 — Turn the mastery and affinity buttons into real spinbuttons

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5b-exhaustive-character-editor-field-coverage.md`
- summary: Story 5b put the current state in the button's accessible name: `aria-label="Fire affinity: Resistant"` and `aria-label="Animal Handling mastery: Expertise"`. Accessibility guidance says the name should only say what the control is, and the state should go in its own attribute. A screen reader may not announce a name that changes on every click. Decrease is right-click only, so keyboard users cannot lower a value.

Proposed direction (chosen over a stable label with `aria-describedby`, and over visible text next to the icon):
- Use `role="spinbutton"` with a stable name (`"Fire affinity"`, `"Animal Handling mastery"`), `aria-valuetext` for the readable state (`"Resistant"`), and `aria-valuenow`/`aria-valuemin`/`aria-valuemax` as numbers kept in sync with the enum.
- Add Up/Down arrow keys to increase and decrease, as the role requires. Keep left-click and right-click for mouse users.
- `role="spinbutton"` is probably not allowed on a `<button>`, so this likely needs a small shared component in place of `MudIconButton`. Check this first.
- Tests: the typed objects use an exact `GetByRole(AriaRole.Spinbutton, Name = ...)` lookup and read `aria-valuetext`. Remove the regex prefix match and `ComponentObject.GetAccessibleNameState`. Add a keyboard test for the arrow keys.

Costs: production behavior changes beyond test needs (new role and keyboard handling), and the numeric value attributes must stay in sync with `SkillMasteryEnum` and `DamageAffinityEnum`.

---

## DW-014 — Split `ApplicationFixture` into a business-free Framework base and a `DndUi.Web` host

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md`
- summary: `uitests/Framework` is meant to hold nothing tied to the application's business, but `ApplicationFixture` composes `DndUi.Web` (`RegisterWebAppServices`, `ConfigureWebAppPipeline`) and its reset/emptiness assertions use domain services (`IFightContext`, `ICharacterRepository`, `ICombatTurnService`, `IAppliedStatusRepository`, `IDnDLogService`). Benoit kept it in Framework for now.

Proposed split: Framework keeps a generic `UiTestFixture` (Playwright browser and page, `CaptureAsync`, artifact folders, the Isolated/Sequential lifecycles) written against a small `IUiTestHost` contract (`StartAsync` returning base URL and `IServiceProvider`, `ResetAsync`, `AssertCleanAsync`, `StopAsync`). A `TestableComponents/Components/TestableDndUi.Web` project would hold `DndUiWebTestHost` (host composition + domain reset) and the `AssemblyFixture` that plugs it in. Framework then drops its `DndUi.Web` reference and `FrameworkReference Microsoft.AspNetCore.App`.

---

## DW-013 — Review findings on the uncommitted Linux-setup work

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/4-screenshot-evidence-capture.md`
- summary: Blind-hunter review of the worktree flagged issues in in-progress changes unrelated to the nested-fixture change.

Evidence: `StateFullNavigation`'s constructor now reads `NavigationManager.Uri`, which throws if the scoped service is resolved before the manager is initialized. Data folders move (`Program.cs`, `LocalFileCharacterRepository`) with no migration of existing data and no guard when `GetFolderPath` returns `""`. Linux `.vscode/tasks.json` pipes `dotnet build/test` through `grep`/`tail` without `pipefail`, so failures report success, and `pkill -f DndUi.Web.dll` can match its own shell.

---

## DW-012 — memlog rewrites line endings

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/4-screenshot-evidence-capture.md`
- summary: Appending to spec-005's `.memlog.md` on Linux converted the file from CRLF to LF, so its diff shows a full rewrite and hides the real additions.

Evidence: there is no `.gitattributes`; a `* text=auto` (or `*.md eol=lf`) rule would normalise line endings across Windows and Linux.

---

## DW-011 — Duplicated package lists across `/uitests` projects

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/4-screenshot-evidence-capture.md`
- summary: `DndUi.SharedUiTests`, `CharacterSheetBlazorComponentsUiTests`, `UiTestFrameworkUiTests` and `Scratch` repeat the same NUnit/Playwright/FluentAssertions package list and linked `AssemblyFixture`/`AssemblyInfo` items; the four `Testable*` projects repeat the same library boilerplate.

Evidence: every new `{OriginProject}UiTests` project copies the block, so versions can drift. A `uitests/Directory.Build.props` could hold the shared packages and links.

---

## DW-010 — `GetScenarioFolder` project-name stripping has no Meta test

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/4-screenshot-evidence-capture.md`
- summary: No Meta test covers the nested-class (`+`) folder split or the strip-up-to-assembly-name rule.

Evidence: when the test class namespace does not contain `.{AssemblyName}.` (e.g. a test method inherited from a base class in another assembly), the method silently falls back to the full namespace as the folder path. A Meta test with a nested fixture asserting its artifact folder would pin both behaviours.

---

## DW-009 — Extend page-object coverage analyzer to component objects

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Each typed object is named `Testable` + the production component it drives and sits at that component's path inside its origin's `uitests/TestableComponents/**/Testable{OriginProject}` project (`TestableCombatLogPanel`, `TestableCombatStatusPanel`, `TestableFighterTile`, `TestableMartialAttackSelector`, `TestableAttackListEditor`, `TestableCharacterMainInfoEditor`, `TestableAttackMainInfoEditor`; production: `CombatLogPanel`, `CombatStatusPanel`, `FighterTile`, `MartialAttackSelector`, `AttackListEditor`, `CharacterMainInfoEditor`, `AttackMainInfoEditor`). Story 7's analyzer only covers `@page` components → page objects.

Consider extending the story-7 analyzer (or a sibling) to also flag production components that have no matching `ComponentObject`, enforcing the 1:1 correspondence at build time. Benoit's related instinct: a test component object with no real production component would be a signal to extract that fragment into a real component — so the analyzer doubles as a nudge toward component extraction. Currently all seven test component objects map to an existing production component, so nothing needs extracting today.

---

## DW-008 — Story-5 fluent surface partially unexercised by scenarios

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Some delivered page/component methods still lack focused runtime evidence. The business-test review added separate player/monster control coverage and first-add initiative interaction; those latest additions have not been collectively runtime-validated.

Evidence: authored tests now cover `FightersPage.SearchPlayers`/`AddPlayer` and `CharacterListEditorPage.EditMonster`/`DuplicateMonster`/`DeleteMonster`, including player removal from Available, monster-template retention and counted monster groups. Do not recreate those cases because this backlog once listed them as absent. Remaining gaps include an explicit `GoToPlayersTab` workflow, `FighterTile.Edit`/`Delete`/`Statuses`, `CombatLogPanel.Entries`/`FightDashboardPage.Log`, and the runtime path of `MartialAttackSelector.SelectAttack`. Story 6 can exercise the log and attack selector while extending the existing first-add initiative helper. Separate UI controls merit separate tests; domain internals and exhaustive rule matrices do not belong in this backlog. See spec-005's `uitests-layout.md` for the current coverage boundary.

The 2026-10-08 refactor separates typed UI actions asserted against real services from independently service-driven rendering cases across all four business test files. Future gap coverage must follow that directionality with no fakes; local search/manual selection remain UI-local. This is authored coverage, not new runtime-pass evidence.

---

## DW-001 — Should `DndUi.Web` stay at all?

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/1-extract-register-web-app-services.md`
- summary: Decide whether the web host is a permanent second host or a temporary agent-screenshot vehicle. Several smaller findings only matter if the answer is "it stays".

Benoit's note: `DndUi.Web` existed mainly so agents could take pictures of the UI, and it may be deleted if SPEC-005 lands well. That decision gates the following, all of which are real but pointless to fix on a host that is about to disappear:

- **Persistence path.** `DndUi.Web` writes characters to `Path.GetTempPath()/DnDFightTool.Web`. The MAUI host uses `LocalAppData\DnDFightTool` (`LocalFileCharacterRepository._defaultFolder`). Temp is cleared by the OS — silent data loss — and on a multi-user machine it is a shared, guessable path another user can pre-create or symlink. `dataFolder` is now a registration parameter, so binding it to configuration with a per-user default is cheap.
- **Composition drift.** The two hosts register an otherwise identical graph, but `DndUi.Web` registers `IValidator<HitRollResult>` and `IValidator<DamageRollResult>` and `MauiProgram.cs` does not. A shared `RegisterDndCoreServices()` would stop the drift.

Note the tension: SPEC-005 targets `DndUi.Web` as the only Playwright rendering path. Deleting the web host would invalidate the whole navigation library, so this is a real fork, not a cleanup task.

---

## DW-004 — Validators registered `Scoped`, documented as `Transient`

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/1-extract-register-web-app-services.md`
- summary: `AddScoped<IValidator<HitRollResult>, …>` and `AddScoped<IValidator<DamageRollResult>, …>` contradict the lifetime convention in `.github/instructions/ioc.instructions.md`.

Plan agreed with Benoit: leave as-is for now, then once SPEC-005 is done and the scenario suite exists, flip both to `Transient` and run the suite to see what breaks. The UI test library is exactly the tool that makes this safe to try — which is why it waits.

---

# Closed

## DW-002 — `UseExceptionHandler("/Error")` pointed at a nonexistent route

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/1-extract-register-web-app-services.md`
- summary: There was no `Error.razor` and no `@page "/Error"` anywhere in the solution, so an unhandled production exception would have 404'd inside the error handler.

Resolved by adding `src/Components/DndUi.Shared/Components/Pages/Error.razor`. It lives in `DndUi.Shared` because that is the only assembly present in **both** the endpoint list (`AddAdditionalAssemblies`) and the `Router`'s list — a page in `DndUi.Web` would get an endpoint but no route match. Static MudBlazor content only, no injected services, so it cannot fail for the same reason the app just did. Verified: `/Error` returns 200 and renders.

---

## DW-003 — Web/MAUI composition drift

- status: dropped
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/1-extract-register-web-app-services.md`
- summary: Folded into DW-001 — only worth acting on if `DndUi.Web` stays.

---

## DW-005 — `UseStaticFiles()` predated .NET 9

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/1-extract-register-web-app-services.md`
- summary: Swapped for `MapStaticAssets()` in `ConfigureWebAppPipeline`, gaining build-time compression and asset fingerprinting support.

Verified serving unchanged: `_content/MudBlazor/MudBlazor.min.css`, `_content/DndUi.Shared/css/app.css` and `_framework/blazor.web.js` all return 200. `App.razor` still uses literal `_content/…` hrefs; switching those to `@Assets["…"]` to actually get fingerprinted URLs is a separate, optional follow-up.

---

## DW-006 — No test covering the service graph

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/1-extract-register-web-app-services.md`
- summary: Extracting composition into a callable method made the container testable, but nothing exercised it.

Resolved by adding `tests/Components/DndUiWebTests/`. `RegisterWebAppServicesTests` builds the real host with `ValidateOnBuild` and `ValidateScopes` enabled (a bare `ServiceCollection` cannot work — `AddRazorComponents` needs the hosting services), asserts characters persist to the supplied `dataFolder`, and covers the null/empty/whitespace guard. 5 tests, all passing.

---

## DW-007 — `UseHttpsRedirection()` versus the story-2 fixture's dynamic port

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/1-extract-register-web-app-services.md`
- summary: Resolved by analysis — no code change needed, which was the cheapest available fix.

`HttpsRedirectionMiddleware` resolves its target port from config, `ASPNETCORE_URLS`, or `IServerAddressesFeature`. When none yields an HTTPS port it logs a warning and passes the request through untouched. An HTTP-only test host therefore works as-is. Story 2's fixture should bind HTTP only and ignore the startup warning; no production change, no test hook.

---

## DW-016 — `StateFullNavigation.NavigateBack()` can fall through to an unreachable URL

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Surfaced while building the story-5 page objects. `StateFullNavigation` records its page history from `NavigationManager.LocationChanged`. If no real in-app `LocationChanged` has fired yet (e.g. a fresh deep-load, or a direct `GotoAsync` in a test), `NavigateBack()` has no seeded history and falls through to a bootstrap URL (observed `https://0.0.0.1/characters`), which is unreachable.

Fixed: the fake `"https://0.0.0.1/characters"` seed is replaced with the circuit's actual initial `NavigationManager.Uri`, captured at construction time (before subscribing to `LocationChanged`, which never fires for the initial load). A direct deep-load into an editor now seeds history with the real landing page, so `NavigateBack()` after `Save`/`Cancel` correctly falls back to `/` instead of the unreachable sentinel.

Renumbered from DW-007, which collided with the story-1 `UseHttpsRedirection()` entry.

---

## DW-015 — Exhaustive character/attack editor field coverage (story 5b)

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Story 5 was narrowed at its spec checkpoint to routable page objects + list/fighters/dashboard component objects + scenarios. The exhaustive field-level editing of the character and attack editors was split off into story `5b`.

Resolved for the character-editor surface (story `5b`): character basic-info deep fields (Max HP, HP, Armor Class, shield toggle + value, effective AC read), `AbilityScoresEditor` (score, save bonus, mastery bonus, save-mastery toggle), `SkillsEditor` (mastery increase/decrease via left/right-click, governing-ability menu), and `ResistancesEditor` (affinity increase/decrease) are now covered by UI-to-real-repository action tests and independently seeded repository-to-UI rendering tests under `uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/`. The attack `DamageRollCollectionEditor` + to-hit modifier editing remains deferred as story `5c` in `stories.yaml`.

Renumbered from DW-006, which collided with the story-1 service-graph entry.

---

## DW-017 — No fast unit test for the `AbilityScoresCollection.MasteryBonus` Mapster fix

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5b-exhaustive-character-editor-field-coverage.md`
- summary: Story 5b fixed a bug where Mapster dropped `MasteryBonus` on `Copy()`/`Clone()` (`AfterMapping` in `src/Domain/CharacterSheet/AbilityScores/Mapping/MappingConfigurations.cs`). Only a UI test covered it.

Resolved 2026-10-23: added `tests/Domain/DnDEntitiesTests/AbilityScores/Mapping/MappingConfigurationsTests.cs`. It checks that Copy and Clone keep `MasteryBonus` and the scores. The two `MasteryBonus` tests fail when the fix is removed. The entry first said no test pattern existed for `Copy`/`Clone`. That was wrong: MartialAttacks and Statuses already had collection mapping tests. Those tests moved to `{Feature}/Mapping/MappingConfigurationsTests.cs`, so each test file now mirrors its source file.

Audited 2026-10-16: no other `List<T>`/`Dictionary<TKey, TValue>` subclass under `src/Domain` has an extra stored property. The standing rule is in [mapping.instructions.md](../../.github/instructions/mapping.instructions.md).
