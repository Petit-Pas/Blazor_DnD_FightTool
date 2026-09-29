# Deferred Work

IDs are stable and never reused. Active entries come first; `resolved` and `dropped`
entries move to the Closed section at the bottom, in ID order. Prune the Closed section
yourself when you no longer want the history.

Status: `open` · `resolved` · `dropped`

---

# Active

## DW-014 — Split `ApplicationFixture` into a business-free Framework base and a `DndUi.Web` host

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md`
- summary: `uitests/Framework` is meant to hold nothing tied to the application's business, but `ApplicationFixture` composes `DndUi.Web` (`RegisterWebAppServices`, `ConfigureWebAppPipeline`) and its reset/emptiness assertions use domain services (`IFightContext`, `ICharacterRepository`, `ICombatTurnService`, `IAppliedStatusRepository`, `IDnDLogService`). Benoit kept it in Framework for now.

Proposed split: Framework keeps a generic `UiTestFixture` (Playwright browser and page, `CaptureAsync`, artifact folders, the Isolated/Sequential lifecycles) written against a small `IUiTestHost` contract (`StartAsync` returning base URL and `IServiceProvider`, `ResetAsync`, `AssertCleanAsync`, `StopAsync`). A `TestableComponents/Components/TestableDndUi.Web` project would hold `DndUiWebTestHost` (host composition + domain reset) and the `AssemblyFixture` that plugs it in. Framework then drops its `DndUi.Web` reference and `FrameworkReference Microsoft.AspNetCore.App`.

---

## DW-013 — Review findings on the uncommitted Linux-setup work

- status: open
- source_spec: `_bmad-output/implementation-artifacts/spec-uitests-nested-fixture-artifact-folders.md`
- summary: Blind-hunter review of the worktree flagged issues in in-progress changes unrelated to the nested-fixture change.

Evidence: `StateFullNavigation`'s constructor now reads `NavigationManager.Uri`, which throws if the scoped service is resolved before the manager is initialized. Data folders move (`Program.cs`, `LocalFileCharacterRepository`) with no migration of existing data and no guard when `GetFolderPath` returns `""`. Linux `.vscode/tasks.json` pipes `dotnet build/test` through `grep`/`tail` without `pipefail`, so failures report success, and `pkill -f DndUi.Web.dll` can match its own shell.

---

## DW-012 — memlog rewrites line endings

- status: open
- source_spec: `_bmad-output/implementation-artifacts/spec-uitests-nested-fixture-artifact-folders.md`
- summary: Appending to spec-005's `.memlog.md` on Linux converted the file from CRLF to LF, so its diff shows a full rewrite and hides the real additions.

Evidence: there is no `.gitattributes`; a `* text=auto` (or `*.md eol=lf`) rule would normalise line endings across Windows and Linux.

---

## DW-011 — Duplicated package lists across `/uitests` projects

- status: open
- source_spec: `_bmad-output/implementation-artifacts/spec-uitests-nested-fixture-artifact-folders.md`
- summary: `DndUi.SharedUiTests`, `CharacterSheetBlazorComponentsUiTests`, `UiTestFrameworkUiTests` and `Scratch` repeat the same NUnit/Playwright/FluentAssertions package list and linked `AssemblyFixture`/`AssemblyInfo` items; the four `Testable*` projects repeat the same library boilerplate.

Evidence: every new `{OriginProject}UiTests` project copies the block, so versions can drift. A `uitests/Directory.Build.props` could hold the shared packages and links.

---

## DW-010 — `GetScenarioFolder` project-name stripping has no Meta test

- status: open
- source_spec: `_bmad-output/implementation-artifacts/spec-uitests-nested-fixture-artifact-folders.md`
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
- summary: Story-5 scenarios cover each I/O-matrix row, but several delivered page/component-object methods have no runtime scenario. Symmetric/low-risk, surfaced by the code-review layer.

Uncovered by a running scenario: `FightersPage.SearchPlayers`/`AddPlayer`, `CharacterListEditorPage.GoToPlayersTab`, `EditMonster`/`DuplicateMonster`/`DeleteMonster` (player variants are covered), `FighterTile.Edit`/`Delete`/`Statuses`, `CombatLogPanel.Entries`/`FightDashboardPage.Log`, and the runtime path of `MartialAttackSelector.SelectAttack`. These are mostly mirror methods of covered ones (player↔monster) or read helpers; the `Cancel` path and the aria-label contract's `Cancel` label are now covered by `Should_Discard_A_New_Character_On_Cancel`. Add opportunistic coverage as later scenarios naturally touch these paths (story 6 will exercise the log and the attack selector). Not a defect — the tested paths pass and the untested ones are symmetric.

---

## DW-006 — Exhaustive character/attack editor field coverage (story 5b)

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Story 5 was narrowed at its spec checkpoint to routable page objects + list/fighters/dashboard component objects + scenarios. The exhaustive field-level editing of the character and attack editors was split off into story `5b`.

Deferred surface: character basic-info deep fields (Max HP, HP, Armor Class, shield toggle + value, effective AC read), `AbilityScoresEditor` (score, save bonus, mastery bonus, save-mastery toggle), `SkillsEditor` (mastery increase/decrease via left/right-click, governing-ability menu), `ResistancesEditor` (affinity increase/decrease), and attack `DamageRollCollectionEditor` + to-hit modifier editing — each with edit-and-re-read scenarios. Tracked as story `5b` in `stories.yaml`; build it on top of story 5's `PageObject`/`ComponentObject` seam (extend, don't duplicate) once story 5 is accepted.

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

## DW-007 — `StateFullNavigation.NavigateBack()` can fall through to an unreachable URL

- status: resolved
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Surfaced while building the story-5 page objects. `StateFullNavigation` records its page history from `NavigationManager.LocationChanged`. If no real in-app `LocationChanged` has fired yet (e.g. a fresh deep-load, or a direct `GotoAsync` in a test), `NavigateBack()` has no seeded history and falls through to a bootstrap URL (observed `https://0.0.0.1/characters`), which is unreachable.

Fixed: the fake `"https://0.0.0.1/characters"` seed is replaced with the circuit's actual initial `NavigationManager.Uri`, captured at construction time (before subscribing to `LocationChanged`, which never fires for the initial load). A direct deep-load into an editor now seeds history with the real landing page, so `NavigateBack()` after `Save`/`Cancel` correctly falls back to `/` instead of the unreachable sentinel.
