# Deferred Work

IDs are stable and never reused. Active entries come first; `resolved` and `dropped`
entries move to the Closed section at the bottom, in ID order. Prune the Closed section
yourself when you no longer want the history.

Status: `open` · `resolved` · `dropped`

---

# Active

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
