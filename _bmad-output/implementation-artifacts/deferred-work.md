# Deferred Work

IDs are stable and never reused. Active entries come first; `resolved` and `dropped`
entries move to the Closed section at the bottom, in ID order. Prune the Closed section
yourself when you no longer want the history.

Status: `open` · `resolved` · `dropped`

---

# Active

## DW-009 — Extend page-object coverage analyzer to component objects

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Story 5 established a convention that each test `ComponentObject` is named `Test` + the production component it drives (`TestCombatLogPanel`, `TestCombatStatusPanel`, `TestFighterTile`, `TestMartialAttackSelector`, `TestAttackListEditor`, `TestCharacterMainInfoEditor`, `TestAttackMainInfoEditor`; production: `CombatLogPanel`, `CombatStatusPanel`, `FighterTile`, `MartialAttackSelector`, `AttackListEditor`, `CharacterMainInfoEditor`, `AttackMainInfoEditor`). Story 7's analyzer only covers `@page` components → page objects.

Consider extending the story-7 analyzer (or a sibling) to also flag production components that have no matching `ComponentObject`, enforcing the 1:1 correspondence at build time. Benoit's related instinct: a test component object with no real production component would be a signal to extract that fragment into a real component — so the analyzer doubles as a nudge toward component extraction. Currently all seven test component objects map to an existing production component, so nothing needs extracting today.

---

## DW-008 — Story-5 fluent surface partially unexercised by scenarios

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Story-5 scenarios cover each I/O-matrix row, but several delivered page/component-object methods have no runtime scenario. Symmetric/low-risk, surfaced by the code-review layer.

Uncovered by a running scenario: `FightersPage.SearchPlayers`/`AddPlayer`, `CharacterListEditorPage.GoToPlayersTab`, `EditMonster`/`DuplicateMonster`/`DeleteMonster` (player variants are covered), `FighterTile.Edit`/`Delete`/`Statuses`, `CombatLogPanel.Entries`/`FightDashboardPage.Log`, and the runtime path of `MartialAttackSelector.SelectAttack`. These are mostly mirror methods of covered ones (player↔monster) or read helpers; the `Cancel` path and the aria-label contract's `Cancel` label are now covered by `Should_Discard_A_New_Character_On_Cancel`. Add opportunistic coverage as later scenarios naturally touch these paths (story 6 will exercise the log and the attack selector). Not a defect — the tested paths pass and the untested ones are symmetric.

---

## DW-007 — `StateFullNavigation.NavigateBack()` can fall through to an unreachable URL

- status: open
- source_spec: `_bmad-output/specs/spec-005-ui-test-navigation-library/stories/5-page-objects-for-routable-pages.md`
- summary: Surfaced while building the story-5 page objects. `StateFullNavigation` records its page history from `NavigationManager.LocationChanged`. If no real in-app `LocationChanged` has fired yet (e.g. a fresh deep-load, or a direct `GotoAsync` in a test), `NavigateBack()` has no seeded history and falls through to a bootstrap URL (observed `https://0.0.0.1/characters`), which is unreachable.

Not fixed here (spec forbids touching production). Worked around in the test library: each page object's `GoToAsync()` reaches its page through a real in-app nav-menu click so the production history is seeded before any editor `Save`/`Cancel` (which call `NavigateBack`). This is latent for real users too: a first interaction that lands directly on an editor and then cancels/saves could hit the same fall-through if the initial `LocationChanged` hasn't registered. Decide whether to make `NavigateBack()` default to `/` (or the home route) when history is empty.

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
