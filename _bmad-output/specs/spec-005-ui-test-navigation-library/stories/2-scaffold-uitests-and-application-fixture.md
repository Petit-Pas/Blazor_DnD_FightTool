---
title: 'Scaffold /uitests and the application fixture'
type: 'feature'
created: '2026-08-28'
status: 'done'
baseline_commit: 'e1847fd227027206e1ca2bddb0eb16c249ae5511'
review_loop_iteration: 0
context:
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\project-context.md'
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\specs\spec-005-ui-test-navigation-library\SPEC.md'
---

<frozen-after-approval reason="human-owned intent — do not modify unless human renegotiates">

## Intent

**Problem:** The repository has no reusable browser-test harness. A UI scenario currently has to know how to launch DndUi.Web, discover its URL, manage Playwright, and clean up the browser, which prevents repeatable rendered smoke tests.

**Approach:** Add a top-level `uitests` area containing a reusable NUnit/Playwright test project, an application fixture, one committed home-page smoke scenario, and an empty scratch-project shell for future agent verification. The fixture composes the real web host and exposes a dynamic, browser-reachable Kestrel URL without changing production behavior.

## Boundaries & Constraints

**Always:** Target `net10.0`; use pinned package versions and project references; use real Kestrel on a dynamically allocated port, not WebApplicationFactory's default TestServer; launch one Chromium browser per assembly; isolate the host's character data in a unique temporary folder per fixture run; disable scenario parallelization; keep test code in English and follow the repository's NUnit, FluentAssertions, nullable, file-scoped namespace, and XML documentation conventions; add both projects to `DnDFightTool.slnx`; ensure the smoke scenario loads the home page and asserts rendered content.

**Ask First:** Any need to alter production host composition, add a test-only endpoint or registration, introduce a package not required for NUnit/Playwright execution, or change the existing HTTPS redirection contract.

**Never:** Do not use bUnit, an in-memory browser transport, fixed ports, MAUI automation, a packaged library, committed scratch scenarios, page objects, dialog objects, teardown undo/delete isolation, screenshot capture, analyzers, or CI integration in this story.

## I/O & Edge-Case Matrix

| Scenario | Input / State | Expected Output / Behavior | Error Handling |
|----------|--------------|---------------------------|----------------|
| HAPPY_PATH | Fixture starts with a unique data folder | Kestrel is reachable, Chromium opens once, and the smoke test sees home-page content | Test fails with the host/browser assertion |
| HOST_START_FAILURE | Port or host startup cannot be established | Fixture initialization fails before the scenario runs | Dispose any partially created browser/host resources |
| FIXTURE_TEARDOWN | Smoke test passes or fails | Browser and host are stopped and temporary resources are disposed | Cleanup must not mask the original test failure |

</frozen-after-approval>

## Code Map

- `src/Components/DndUi.Web/Program.cs` -- production entry point; already delegates service registration and pipeline composition to public extensions.
- `src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs` -- `RegisterWebAppServices(IServiceCollection, string)` accepts the fixture's isolated `dataFolder` and registers the real singleton graph.
- `src/Components/DndUi.Web/Hosting/WebApplicationExtensions.cs` -- `ConfigureWebAppPipeline(WebApplication)` maps the Blazor app and currently applies HTTPS redirection; fixture URL configuration must account for this.
- `tests/Components/DndUiWebTests/DndUiWebTests.csproj` -- nearby NUnit project pattern and pinned test package versions; do not modify this project.
- `DnDFightTool.slnx` -- solution project listing; add the reusable and scratch projects beneath a `/uitests/` folder.
- `.gitignore` -- existing build/artifact exclusions; extend narrowly for scratch contents and future UI-test artifacts without ignoring committed project files.
- `uitests/UiTestNavigation/UiTestNavigation.csproj` -- new NUnit/Playwright library test project; reference `DndUi.Web` and pin `Microsoft.Playwright`.
- `uitests/UiTestNavigation/ApplicationFixture.cs` -- new assembly-level fixture that builds the real host with a unique temp data folder, binds a dynamic Kestrel endpoint, and owns one browser lifecycle.
- `uitests/UiTestNavigation/HomePageSmokeTests.cs` -- committed smoke scenario using only fixture-provided browser/page access and semantic assertions.
- `uitests/Scratch/Scratch.csproj` -- committed empty test shell referencing the reusable project; scratch source and output are ignored.

## Tasks & Acceptance

**Execution:**
- [x] `uitests/UiTestNavigation/UiTestNavigation.csproj` -- create the pinned NUnit/Playwright test project with the web-host project reference -- provide the reusable test boundary.
- [x] `uitests/UiTestNavigation/ApplicationFixture.cs` -- implement real Kestrel startup on a dynamic reachable endpoint, unique temp data-folder composition through existing extensions, one Chromium launch per assembly, and deterministic disposal -- hide infrastructure from scenarios.
- [x] `uitests/UiTestNavigation/HomePageSmokeTests.cs` -- add a smoke test that derives from the fixture and asserts home-page rendered content without launch, port, URL, or browser setup code -- prove the harness works.
- [x] `uitests/Scratch/Scratch.csproj` -- add an empty non-packable test shell referencing the library -- establish the agent scratch entry point.
- [x] `DnDFightTool.slnx` -- register both projects under `/uitests/` -- make the new projects buildable from the solution.
- [x] `.gitignore` -- ignore scratch scenario contents and UI-test artifacts while retaining project files -- prevent throwaway output from entering source control.

**Acceptance Criteria:**
- Given the solution is built, when the new projects are included, then both projects restore and compile successfully.
- Given Chromium is installed, when the smoke test runs, then it starts DndUi.Web on a dynamic real socket, loads the home page, and passes a rendered-content assertion.
- Given multiple smoke tests execute in one assembly, when the fixture lifecycle runs, then Chromium is launched once and all host/browser resources are disposed after the assembly.
- Given the scratch shell contains temporary scenarios or screenshots, when git status is inspected, then those contents are ignored while the shell project remains tracked.

## Verification

**Commands:**
- `dotnet build DnDFightTool.slnx` -- expected: SUCCESS.
- `dotnet test uitests/UiTestNavigation/UiTestNavigation.csproj` -- expected: smoke scenario passes.
- `dotnet test uitests/Scratch/Scratch.csproj` -- expected: project discovers no committed scenarios and exits successfully.

## Suggested Review Order

**Host and fixture lifecycle**

- The fixture composes the production host while supplying a real dynamic Kestrel endpoint.
  [ApplicationFixture.cs:52](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L52)

- Assembly setup owns one browser lifecycle and links cleanly into scratch test assemblies.
  [AssemblyFixture.cs:8](../../../../uitests/UiTestNavigation/AssemblyFixture.cs#L8)

- Cleanup releases browser and host resources and removes the isolated temporary data folder.
  [ApplicationFixture.cs:96](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L96)

**Test entry points and project wiring**

- The smoke scenario proves a derived test can reach rendered home-page content without launch code.
  [HomePageSmokeTests.cs:10](../../../../uitests/UiTestNavigation/HomePageSmokeTests.cs#L10)

- Package and project references define the reusable NUnit/Playwright test boundary.
  [UiTestNavigation.csproj:1](../../../../uitests/UiTestNavigation/UiTestNavigation.csproj#L1)

- The scratch shell reuses lifecycle setup while keeping temporary scenarios uncommitted.
  [Scratch.csproj:13](../../../../uitests/Scratch/Scratch.csproj#L13)

- Solution registration and ignore rules expose the new area without tracking generated output.
  [DnDFightTool.slnx:61](../../../../DnDFightTool.slnx#L61)

- Scratch scenarios and UI-test artifacts remain outside source control.
  [.gitignore:65](../../../../.gitignore#L65)
