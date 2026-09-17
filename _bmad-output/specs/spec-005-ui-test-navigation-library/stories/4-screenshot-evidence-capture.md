---
title: 'Screenshot evidence capture'
type: 'feature'
created: '2026-09-15'
status: 'done'
baseline_commit: 'bf324bc99975fdac7974eb0f8eba81820c8b9299'
review_loop_iteration: 0
context:
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\project-context.md'
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\specs\spec-005-ui-test-navigation-library\SPEC.md'
---

<frozen-after-approval reason="human-owned intent — do not modify unless human renegotiates">

## Intent

**Problem:** The scenario harness (stories 1–3) can drive the app but produces no visual evidence. CAP-6 requires that a reader see the feature working: a scenario must be able to capture a named screenshot at any point, and every scenario must leave a final-state image behind — for passing runs, not only failures — so agent-driven UI work is provable without a human re-clicking through it.

**Approach:** Add a `CaptureAsync(name)` API on the shared `ApplicationFixture` base that writes a full-page PNG into a per-scenario folder under a gitignored `artifacts/` root, and have the scenario fixture's `[TearDown]` take an automatic `final` capture as its very first action — before the state reset empties the screen. Files are named by scenario and step so the sequence reads top-to-bottom without opening each image. Because the `Scratch` project links the fixtures, scratch scenarios inherit both the named-capture API and the automatic final image for free, and their screenshots land under the scratch project's own gitignored `artifacts/` folder.

## Boundaries & Constraints

**Always:** Screenshots use `IPage.ScreenshotAsync` with `FullPage = true`. The artifacts root resolves per running test project so each project keeps its own evidence: derive it from the running test assembly location (`AppContext.BaseDirectory`) by walking up to the nearest folder containing a `.csproj`, then `artifacts` — so library scenarios write to `uitests/UiTestNavigation/artifacts/` and scratch scenarios to `uitests/Scratch/artifacts/`. Never resolve from the process working directory. Each scenario nests under a folder path that mirrors its namespace below the fixture's namespace, then the test name (whatever that namespace happens to be), each segment sanitized to filesystem-safe characters, so the layout matches the test explorer. Files are named `NN-{name}.png` where `NN` is a zero-padded per-scenario step index that increments on every capture (named or automatic); the index is reset in the fixture's `[SetUp]`. The automatic `final` capture happens as the first statement of the fixture's `[TearDown]`, before the state reset, so the image reflects the scenario's end state rather than the emptied slate. Clear the scenario's subfolder in the fixture's `[SetUp]` so a re-run does not mix stale step files with new ones. Follow the repository's NUnit, FluentAssertions, nullable, file-scoped-namespace, and XML-doc conventions.

**Ask First:** Any need to add a screenshot dependency, change the `artifacts/` location or the gitignore, or make the final capture a hard failure when it throws.

**Never:** No baseline images, no image diffing, no failing a run on visual drift. Do not gate captures on test outcome — passing runs produce images too. Do not write screenshots outside the gitignored `artifacts/` tree, and do not add the images to version control. Do not add a separate capture API to the scratch project — it inherits `CaptureAsync` through the linked fixture. Do not let a failed automatic `final` capture prevent the isolation teardown (undo + character cleanup) from running. Do not touch `MauiProgram.cs` or production hosting.

## I/O & Edge-Case Matrix

| Scenario | Input / State | Expected Output / Behavior | Error Handling |
|----------|--------------|---------------------------|----------------|
| NAMED_CAPTURE | Scenario calls `CaptureAsync("home")` after loading a page | `00-home.png` (full-page) written under the scenario's mirrored-namespace folder | Playwright/IO error propagates — the author asked for this shot |
| AUTOMATIC_FINAL | Any scenario completes | Teardown writes the next-indexed `NN-final.png` before undoing commands | Capture is best-effort; on error, isolation teardown still runs |
| STEP_SEQUENCE | Scenario captures three named shots then finishes | Files `00-…`, `01-…`, `02-…`, `03-final.png` in call order | — |
| STALE_RERUN | Scenario re-run after its step names changed | `[SetUp]` clears the subfolder before the run; only the current run's files remain | — |
| UNSAFE_NAME | Parameterized test name or capture name with path characters | Name sanitized to filesystem-safe characters before use | — |
| SCRATCH_CAPTURE | A scratch scenario (linked fixture) calls `CaptureAsync` or completes | Image lands under `uitests/Scratch/artifacts/`, gitignored | Same best-effort/strict split as library scenarios |

</frozen-after-approval>

## Code Map

- `uitests/UiTestNavigation/ApplicationFixture.cs` -- abstract shared base (host/browser/`Page`, `CaptureAsync`, `GetScenarioFolder`, `PrepareScenarioArtifacts`, `CaptureFinalStateAsync`, `ResetApplicationStateAsync`, `AssertCommandDrivenStateIsEmpty`). Declares no `[SetUp]`/`[TearDown]`; not to be derived directly. Also linked into `Scratch`.
- `uitests/UiTestNavigation/IsolatedScenarioFixture.cs` -- independent-scenario base: `[SetUp]` clears the screenshot folder, `[TearDown]` captures `final` then resets all state (undo everything + delete characters). All current Meta tests derive from this.
- `uitests/UiTestNavigation/SequentialScenarioFixture.cs` -- ordered-chain base: heavy reset only in `[OneTimeSetUp]`/`[OneTimeTearDown]` so steps share state; per-step `[SetUp]` clears that step's folder; per-step `[TearDown]` captures `final` and flags failure; a failed step skips the rest (`Assert.Ignore`). Ready for the multi-step combat scenarios of later stories; no scenario derives from it yet.
- `uitests/UiTestNavigation/AssemblyInfo.cs` -- `FixtureLifeCycle` attribute removed; NUnit's default one-instance-per-fixture is what both fixtures use, since state resets live in `[SetUp]`/`[TearDown]`.
- `uitests/UiTestNavigation/Meta/ScreenshotEvidenceTests.cs` -- NEW. Framework-validation scenarios under `Meta/`: NAMED_CAPTURE, STEP_SEQUENCE, UNSAFE_NAME, STALE_RERUN, plus the automatic-final assertion in `[OneTimeTearDown]`.
- `uitests/UiTestNavigation/Meta/FinalCaptureFailureTests.cs` -- NEW. AC4 error-path proof: overrides `CaptureAsync` to throw and arranges real state, so it passes only if the best-effort catch lets the isolation reset still run.
- `uitests/UiTestNavigation/Extensions/FileExtensions.cs` -- NEW. Holds the file-system mechanics (`DirectoryInfo.FindAncestorContaining("*.csproj")`, `string.ToFileSafeName()`) so the fixture stays free of directory-walking and char-sanitization detail.
- `.gitignore` -- already ignores `uitests/**/artifacts/` (lines 66–67), which covers both `uitests/UiTestNavigation/artifacts/` and `uitests/Scratch/artifacts/`; also ignores `uitests/Scratch/**/*.png`. No change; do not add the images to VCS.
- `uitests/UiTestNavigation/AssemblyFixture.cs` -- unchanged; the single host/browser lifecycle already spans the assembly. No change.

## Tasks & Acceptance

**Execution:**
- [x] `uitests/UiTestNavigation/ApplicationFixture.cs` -- add `CaptureAsync(name)`, the per-project artifacts-root resolver (walk up to the nearest `.csproj`), the per-scenario folder resolver (namespace tree below the fixture then `{TestName}`, sanitized), the per-instance step index, and the first-capture folder-clear; call the automatic `final` capture (best-effort, swallowing errors) as the first statement of `CleanUpScenarioAsync` before the undo loop -- gives scenarios named evidence and guarantees a final image without breaking isolation.
- [x] `uitests/UiTestNavigation/Meta/ScreenshotEvidenceTests.cs` -- add NAMED_CAPTURE and STEP_SEQUENCE scenarios that drive a page, capture, and assert the artifact files exist with the expected ordered names including the automatic `final` -- proves the I/O matrix rows on disk.

**Acceptance Criteria:**
- Given a scenario that loads a page and calls `CaptureAsync("home")`, when it runs and passes, then a full-page `00-home.png` exists under the scenario's mirrored-namespace folder within the project's `artifacts/` tree.
- Given any passing scenario, when its teardown runs, then a final `NN-final.png` exists in that scenario's folder captured before the undo-everything cleanup, and the isolation teardown (undo + character deletion + emptiness assertions from story 3) still completes.
- Given a scenario captures several named shots, when it finishes, then the folder holds sequentially numbered files in call order ending with the automatic `final`.
- Given the automatic final capture throws, when teardown runs, then the isolation cleanup still runs and the scenario's own result is unaffected by the screenshot failure.

## Design Notes

**Fixture architecture (human renegotiation).** `ApplicationFixture` is an abstract base holding shared plumbing and reusable helpers (`CaptureAsync`, `PrepareScenarioArtifacts`, `CaptureFinalStateAsync`, `ResetApplicationStateAsync`) but no `[SetUp]`/`[TearDown]` and no lifecycle opinion. Two concrete bases derive from it: `IsolatedScenarioFixture` resets all state per scenario (`[SetUp]` clears the folder, `[TearDown]` captures `final` then resets), so scenarios are independent; `SequentialScenarioFixture` shares state across `[Order]`ed steps and resets only at the fixture boundary (`[OneTimeSetUp]`/`[OneTimeTearDown]`), captures a `final` per step, and skips the remaining steps once one fails (`Assert.Ignore`). Because folder-clear and step-index reset moved into `[SetUp]`, the assembly-wide `FixtureLifeCycle(InstancePerTestCase)` was removed — NUnit's default one-instance-per-fixture is exactly what the sequential fixture needs (a persistent fail-fast flag), and the isolated fixture is unaffected because everything resets in setup/teardown.

**Teardown ordering is the crux.** The state reset undoes every command, which empties the visible UI. The automatic `final` capture must therefore be the first statement of the fixture's `[TearDown]`, before the reset — otherwise "final state" is a blank page. Named captures inside the scenario body are unaffected.

**Strict named vs best-effort final.** An explicit `CaptureAsync` call is something the author asked for, so let its errors propagate. The automatic `final` is a convenience; wrap it so a screenshot failure never masks the real test result nor blocks the isolation teardown that keeps scenarios independent.

**Per-project artifacts root.** The root is resolved from the running test assembly (`AppContext.BaseDirectory`) by walking up to the nearest directory containing a `.csproj`, then appending `artifacts`. This keeps each project's evidence separate — committed library scenarios in `uitests/UiTestNavigation/artifacts/`, throwaway scratch runs in `uitests/Scratch/artifacts/` — without depending on the runner's working directory, and both roots already match the gitignore. Example shape:

```csharp
private static string GetArtifactsRoot()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null && !dir.EnumerateFiles("*.csproj").Any())
    {
        dir = dir.Parent;
    }

    return Path.Combine(dir!.FullName, "artifacts");
}
```

**Scratch inherits capture.** The `Scratch` project links `ApplicationFixture`, so a scratch scenario deriving from it already has `CaptureAsync` and the automatic `final` image. The per-project root above sends its screenshots to `uitests/Scratch/artifacts/`, which is what the agent-verification workflow (CAP-7) cites as evidence. No capture code is added to the scratch project itself.

**Folder layout (human renegotiation).** Scenarios mirror the namespace tree below the fixture's namespace, then the test name, rather than one flat `{ClassName}.{TestName}` folder — so the on-disk layout matches the test explorer. `Meta/` only appears because the current tests live there; it is not enforced. The project-level namespace prefix is dropped because it is redundant with the per-project artifacts root. Directory-walk and name-sanitization mechanics live in `Extensions/FileExtensions.cs`, not the fixture.

**Static assets must serve, or the evidence is worthless.** The first screenshots came out completely unstyled — `MapStaticAssets` matched the CSS routes but returned 0-byte bodies with no `text/css`. Cause: `ApplicationFixture.StartApplicationAsync` set `ApplicationName`/`EnvironmentName` by mutating `builder.Environment` *after* `WebApplication.CreateBuilder()`. The Development static-web-assets loader reads `ApplicationName` during host build to locate `DndUi.Web`'s asset manifest, so setting it late left it keyed to the test-runner assembly. Fix: pass both through `WebApplicationOptions` at `CreateBuilder`. Verified the scoped bundle and `_content/MudBlazor/MudBlazor.min.css` now return real `text/css` content and screenshots render the themed UI.

## Verification

**Commands:**
- `dotnet build DnDFightTool.slnx` -- expected: SUCCESS.
- `dotnet test uitests/UiTestNavigation/UiTestNavigation.csproj` -- expected: the new screenshot scenarios pass alongside the existing smoke/isolation ones.

**Manual checks:**
- After the run, the test project's `artifacts/` folder (`uitests/UiTestNavigation/artifacts/`, or `uitests/Scratch/artifacts/` for scratch runs) mirrors the scenario namespace tree, each leaf holding sequentially numbered PNGs ending in `*-final.png`, and none of them are tracked by git.

## Suggested Review Order

**Fixture architecture**

- Abstract shared base — helpers only, no lifecycle; not to be derived directly.
  [`ApplicationFixture.cs:82`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L82)

- Isolated base — `[SetUp]` clears the folder, `[TearDown]` captures `final` then resets all state.
  [`IsolatedScenarioFixture.cs:29`](../../../../uitests/UiTestNavigation/IsolatedScenarioFixture.cs#L29)

- Sequential base — shared state, per-step `final`, fail-fast skip; heavy reset only at the fixture boundary.
  [`SequentialScenarioFixture.cs:32`](../../../../uitests/UiTestNavigation/SequentialScenarioFixture.cs#L32)

**Capture API and evidence flow (the core)**

- Entry point: the named-capture API — full-page PNG, `NN-{name}.png`.
  [`ApplicationFixture.cs:165`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L165)

- The crux: best-effort `final` capture, called first in each fixture's teardown before the reset.
  [`ApplicationFixture.cs:99`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L99)

- The reusable state reset (undo-all + character deletion + emptiness proof).
  [`ApplicationFixture.cs:118`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L118)

**Path resolution**

- Per-scenario folder mirrors the namespace tree; `protected` so a test can plant/inspect its own files.
  [`ApplicationFixture.cs:188`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L188)

- File-system mechanics extracted out of the fixture.
  [`FileExtensions.cs:18`](../../../../uitests/UiTestNavigation/Extensions/FileExtensions.cs#L18)

**Scenarios (the proof)**

- NAMED_CAPTURE and STEP_SEQUENCE plus the automatic-final assertion in `[OneTimeTearDown]`.
  [`ScreenshotEvidenceTests.cs:21`](../../../../uitests/UiTestNavigation/Meta/ScreenshotEvidenceTests.cs#L21)

- STALE_RERUN: a planted leftover is erased by the `[SetUp]` preparation.
  [`ScreenshotEvidenceTests.cs:86`](../../../../uitests/UiTestNavigation/Meta/ScreenshotEvidenceTests.cs#L86)

- AC4 best-effort: forcing the final capture to throw still completes the isolation reset.
  [`FinalCaptureFailureTests.cs:36`](../../../../uitests/UiTestNavigation/Meta/FinalCaptureFailureTests.cs#L36)
