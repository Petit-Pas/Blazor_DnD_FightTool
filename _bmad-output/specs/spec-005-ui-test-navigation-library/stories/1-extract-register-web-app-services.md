---
title: 'Extract DndUi.Web composition into RegisterWebAppServices / ConfigureWebAppPipeline'
type: 'refactor'
created: '2026-08-26'
status: 'done'
baseline_commit: '7ccda678f36e7f8156aad4d0c06fc5b82013cece'
review_loop_iteration: 0
context:
  - '{project-root}/_bmad-output/project-context.md'
  - '{project-root}/.github/instructions/ioc.instructions.md'
  - '{project-root}/_bmad-output/specs/spec-005-ui-test-navigation-library/SPEC.md'
---

<frozen-after-approval reason="human-owned intent — do not modify unless human renegotiates">

## Intent

**Problem:** `DndUi.Web/Program.cs` composes the whole web host inline — service registrations and the middleware/endpoint pipeline. SPEC-005's Playwright fixture must start the *same* host on real Kestrel, and today it has no way to do that without copy-pasting the composition, which would silently drift from production.

**Approach:** Move every service registration into `RegisterWebAppServices(services, dataFolder)` and the middleware plus endpoint mapping into `ConfigureWebAppPipeline(app)`, both in `DndUi.Web`, following the existing `IoC/ServiceCollectionExtensions` convention. `Program.cs` becomes builder → register → build → configure → run. Pure refactor: no behaviour change.

## Boundaries & Constraints

**Always:**
- Preserve every registration's service type, implementation type, lifetime, and **relative order** exactly as in today's `Program.cs`.
- `dataFolder` stays a parameter of `RegisterWebAppServices` so the fixture can pass its own temp folder; `Program.cs` keeps supplying `Path.Combine(Path.GetTempPath(), "DnDFightTool.Web")`.
- Pipeline order and the `IsDevelopment()` branch stay byte-for-byte equivalent in behaviour.
- Both methods are `public` and return their input for chaining (`IServiceCollection` / `WebApplication`).
- Obey `project-context.md` C# rules: file-scoped namespace, no primary constructors, no expression-bodied members, XML docs on public members.

**Ask First:**
- Adding, removing, or re-lifetiming any registration.
- Introducing a new abstraction, options object, or parameter beyond `dataFolder`.
- Touching `MauiProgram.cs`.

**Never:**
- Change `MauiProgram.cs` — the MAUI host registers a deliberately different set; leave it alone.
- Add test-only hooks, conditional flags, or `#if` blocks to production composition.
- Create the `/uitests` folder or any test project — that is story 2.
- Move the composition into a shared project. It stays inside `DndUi.Web`.

</frozen-after-approval>

## Code Map

- `src/Components/DndUi.Web/Program.cs` — the only file being emptied. Services block: `AddRazorComponents().AddInteractiveServerComponents()`, `AddMudServices()`, `AddValidatorsFromAssemblyContaining<CharacterValidator>()`, the two explicit `IValidator<HitRollResult>` / `IValidator<DamageRollResult>` scoped registrations, `dataFolder` + `ICharacterRepository` factory lambda, singletons (`IFightContext`, `ICombatTurnService`, `IAppliedStatusRepository`, `IFileManager`, `IJsonSerializer`, `IMapper`), `ConfigureMediator` with `ShouldScanAutomatically = false` and the two scanned assemblies, the `Register*Services()` chain, `IDialogServiceProvider`. Pipeline block: `UseExceptionHandler("/Error")` + `UseHsts()` under `!IsDevelopment()`, `UseHttpsRedirection`, `UseStaticFiles`, `UseAntiforgery`, `MapRazorComponents<App>().AddInteractiveServerRenderMode().AddAdditionalAssemblies(typeof(Routes).Assembly)`.
- `src/UI/FightBlazorComponents/IoC/ServiceCollectionExtensions.cs` — reference shape for the new class (public static class, `Register…Services`, returns `services`).
- `src/Components/DndUi.Web/DndUi.Web.csproj` — `RootNamespace` is `DnDFightTool.Components.DndUi.Web`; new files live under it. No csproj change needed.
- `src/Components/DndUi/MauiProgram.cs` — read-only. Confirms the MAUI host is a separate composition; do not unify.
- `_bmad-output/specs/spec-005-ui-test-navigation-library/SPEC.md` — the constraint driving this ("production refactor so the fixture can compose the same host — not a test hook inside it").

## Tasks & Acceptance

**Execution:**
- [x] `src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs` — new `public static class ServiceCollectionExtensions` with `RegisterWebAppServices(this IServiceCollection services, string dataFolder)`; move all registrations verbatim, preserving order; return `services`.
- [x] `src/Components/DndUi.Web/IoC/WebApplicationExtensions.cs` — new `public static class WebApplicationExtensions` with `ConfigureWebAppPipeline(this WebApplication app)`; move middleware and `MapRazorComponents` verbatim, preserving order; return `app`.
- [x] `src/Components/DndUi.Web/Program.cs` — reduce to `CreateBuilder` → `RegisterWebAppServices(dataFolder)` → `Build()` → `ConfigureWebAppPipeline()` → `Run()`; drop now-unused `using` directives.

**Acceptance Criteria:**
- Given the refactored host, when `DndUi.Web` is built, then it compiles with no new warnings and `Program.cs` contains no `builder.Services.*` or `app.Use*` / `app.Map*` calls.
- Given the app is run, when the home page is opened and a fight dashboard action is performed, then behaviour is identical to before the refactor (interactive circuit connects, MudBlazor renders, dialogs open, characters persist to the temp data folder).
- Given a caller other than `Program.cs`, when it calls `RegisterWebAppServices(services, someOtherFolder)`, then `ICharacterRepository` resolves against `someOtherFolder` with no other registration differing.
- Given `MauiProgram.cs`, when this story is complete, then it is unchanged.

## Spec Change Log

## Design Notes

`ICharacterRepository` is registered through a factory lambda that closes over `dataFolder`; keep it a lambda closing over the **parameter**, not a captured local, so each call composes against the folder it was given.

`ConfigureMediator`'s `AssembliesToScan` uses `typeof(CasterCommandBase).Assembly` and `typeof(SaveRollResultQueryHandler).Assembly` as assembly anchors — carry those exact type references over rather than substituting other types from the same assemblies.

`UseHttpsRedirection()` moves as-is. It may need revisiting when the fixture binds a dynamic HTTP port in story 2, but changing it here would be an unrequested behaviour change.

**Finding for downstream stories (pre-existing, unchanged by this refactor):** `AddAdditionalAssemblies` registers only `DndUi.Shared`, so only `/`, `/Characters`, `/fight-dashboard` and `/fighters` are server-routable. `/Characters/Edit` and `/Attacks/Edit` live in `CharacterSheetBlazorComponents` and return 404 on a direct request — they are reachable only by client-side navigation inside the circuit. Page objects for those two must navigate through the app, not by URL.

## Verification

**Commands:**
- `dotnet build src/Components/DndUi.Web/DndUi.Web.csproj` — expected: build succeeded, 0 errors.
- `dotnet build DnDFightTool.slnx` — expected: solution-wide build succeeded (confirms the MAUI host still compiles).
- `git diff --stat` — expected: exactly three files touched under `src/Components/DndUi.Web/`.

**Manual checks (if no CLI):**
- Run `dotnet run --project src/Components/DndUi.Web/DndUi.Web.csproj`, open the home page, navigate to the fight dashboard, create or edit a character, and trigger an attack roll dialog — all must work as before.

## Suggested Review Order

**Service composition**

- The whole service graph, moved verbatim; read this against the old `Program.cs` to confirm nothing changed.
  [`ServiceCollectionExtensions.cs:40`](../../../../src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs#L40)

- Fail-fast guard added in review: an empty `dataFolder` would otherwise surface on first request, not at startup.
  [`ServiceCollectionExtensions.cs:42`](../../../../src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs#L42)

- The `dataFolder` seam story 2 depends on — a factory closing over the parameter, not a captured local.
  [`ServiceCollectionExtensions.cs:55`](../../../../src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs#L55)

- Mediator assembly anchors carried over as the exact same `typeof(...)` references.
  [`ServiceCollectionExtensions.cs:64`](../../../../src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs#L64)

**Pipeline composition**

- Middleware and endpoint mapping, moved verbatim; filed outside `IoC/` because it composes no services.
  [`WebApplicationExtensions.cs:17`](../../../../src/Components/DndUi.Web/Hosting/WebApplicationExtensions.cs#L17)

**Entry point**

- 83 lines down to 12 — the shape story 2's fixture will mirror.
  [`Program.cs:1`](../../../../src/Components/DndUi.Web/Program.cs#L1)
