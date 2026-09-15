---
title: 'Scenario isolation and teardown'
type: 'feature'
created: '2026-09-15'
status: 'done'
baseline_commit: '62c74cfc513be6d10c02cfcba5ddce6399530c10'
review_loop_iteration: 0
context:
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\project-context.md'
  - 'D:\Code\Perso\DnDFightTool\_bmad-output\specs\spec-005-ui-test-navigation-library\SPEC.md'
---

<frozen-after-approval reason="human-owned intent — do not modify unless human renegotiates">

## Intent

**Problem:** Scenarios share one host and its singletons (`IFightContext`, the command history, the log, the character repository). Without cleanup between scenarios, state from one leaks into the next, and the suite cannot run in a randomised order.

**Approach:** Add a per-scenario teardown on `ApplicationFixture` that undoes every command while `HistoryLength > 0`, then deletes any remaining characters through the existing repository API. The teardown asserts the command-driven state is genuinely empty afterwards, so a broken undo fails the scenario that caused it rather than corrupting a later one. The test host raises `CommandHistoryMaxSize` so no realistic scenario overflows the history.

## Boundaries & Constraints

**Always:** Undo via `IUndoableMediator.UndoLastCommandAsync()` in a loop while `HistoryLength > 0`, resolving the singleton mediator from the running host's `IServiceProvider`. After the loop, assert command-driven state is empty: `IFightContext.Fighters`, `ICombatTurnService.IsStarted`/`TurnOrder`, and the applied-status repository are all empty. For the log — whose undo hides entries rather than removing them (`WriteLog.Undo` → `Hide`, `OpenBlock.Undo` → `CloseBlock`, neither removes from `Blocks`) — assert no visible entry remains (proving undo hid everything), then `IDnDLogService.Clear()` it so the next scenario starts genuinely empty. Delete leftover characters via `ICharacterRepository.GetAllCharacters()` + `Delete()`, then assert `Count == 0`. Set `CommandHistoryMaxSize` to 1000 in the test host composition only. Framework-validation tests (those that prove the harness itself works, not the application) live under a `Meta/` folder. Follow the repository's NUnit, FluentAssertions, nullable, file-scoped-namespace, and XML-doc conventions.

**Ask First:** Any need to add a `Reset()` to a service, add a test-only registration/endpoint to the production host, or change command/undo semantics.

**Never:** No `Reset()` on any service. No per-scenario host rebuild. No test-only backdoor in the production host. Do not wipe characters by deleting the data folder — go through the repository business API. Do not touch `MauiProgram.cs`.

## I/O & Edge-Case Matrix

| Scenario | Input / State | Expected Output / Behavior | Error Handling |
|----------|--------------|---------------------------|----------------|
| CLEAN_AFTER_COMBAT | A prior scenario left fighters, log entries and history | Teardown undoes every command, state asserts empty, characters deleted, `Count == 0` | — |
| ISOLATION | Two sequential scenarios; the first mutates state | The second starts from an empty fight, empty log and empty repository | — |
| BROKEN_UNDO | Undoing a command leaves residual command-driven state | Teardown's emptiness assertion fails, attributing the failure to the offending scenario | Assertion failure surfaces in that scenario's teardown |
| UNDO_REDO_SUBJECT | A scenario whose own subject is undo/redo | It opts out of the undo-everything teardown and owns its own history cleanup | — |

</frozen-after-approval>

## Code Map

- `src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs` -- `RegisterWebAppServices(services, dataFolder)`; `ConfigureMediator` is called here without setting `CommandHistoryMaxSize` (package default 64). Add an optional `int commandHistoryMaxSize = 256` parameter and feed it into `options.CommandHistoryMaxSize`. Production default becomes 256 (human renegotiation, see Design Notes); the fixture passes 1000.
- `src/Components/DndUi.Web/Program.cs` -- calls `RegisterWebAppServices(dataFolder)`; unchanged, inherits the 256 default. Verify it still boots.
- `src/Domain/Fight/IAppliedStatusRepository.cs` -- has no global read surface, only `GetStatusAppliedTo(id)`. Add `IEnumerable<AppliedStatus> AppliedStatuses { get; }`, mirroring `IFightContext.Fighters`, so teardown can assert emptiness.
- `src/Domain/Fight/AppliedStatusRepository.cs` -- `: Dictionary<Guid, AppliedStatus>`; implement `AppliedStatuses` returning `Values`.
- `src/Domain/Logs/DnDLogService.cs` / `src/Domain/Logs/LogBlock.cs` -- `Blocks` (each with `Entries`), `IsHidden(id)`, and the existing `Clear()`; the hide-based undo is why teardown asserts no-visible-entry then clears rather than asserting `Blocks` empty directly.
- `uitests/UiTestNavigation/ApplicationFixture.cs` -- pass `1000` to `RegisterWebAppServices`; expose the host `IServiceProvider`; add an instance `[TearDown]` running the undo loop, emptiness assertions (including the log no-visible-entry + `Clear()`) and character deletion; add `protected virtual bool UndoAllCommandsOnTeardown { get; } = true;` for opt-out.
- `src/Domain/Fight/IFightContext.cs` (`Fighters`), `src/Domain/Logs/IDnDLogService.cs` (`Blocks`, `IsHidden`, `Clear`), `src/Domain/Fight/TurnTracking/ICombatTurnService.cs` (`IsStarted`, `TurnOrder`), `src/Domain/CharacterSheet/Characters/ICharacterRepository.cs` (`GetAllCharacters`, `Delete`, `Count`) -- read-only anchors for the teardown assertions and cleanup.
- `uitests/UiTestNavigation/Meta/HomePageSmokeTests.cs` -- existing fixture-derived scenario, relocated under `Meta/`; the reference for how scenarios reach `Page`/`BaseUrl`.

## Tasks & Acceptance

**Execution:**
- [x] `src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs` -- add optional `commandHistoryMaxSize = 256` param, set `options.CommandHistoryMaxSize` from it -- lets the fixture raise the ceiling to 1000 while production defaults to 256, one composition path.
- [x] `src/Domain/Fight/IAppliedStatusRepository.cs` + `src/Domain/Fight/AppliedStatusRepository.cs` -- add `IEnumerable<AppliedStatus> AppliedStatuses { get; }` returning `Values` -- gives teardown a surface to assert the applied-status state is empty.
- [x] `uitests/UiTestNavigation/ApplicationFixture.cs` -- pass `1000`; expose host `IServiceProvider`; add `[TearDown]` (undo loop while `HistoryLength > 0`, call `AssertCommandDrivenStateIsEmpty()` — for the log assert no visible entry then `Clear()` — delete characters, assert `Count == 0`) gated by `UndoAllCommandsOnTeardown`; extract the emptiness check into the reusable `AssertCommandDrivenStateIsEmpty()`; add the virtual opt-out property -- makes every scenario start from a proven-clean slate.
- [x] `uitests/UiTestNavigation/Meta/ScenarioIsolationTests.cs` -- scenarios under `Meta/`: clean-slate, empty-start, logging-command (hide-then-clear log path), and a BROKEN_UNDO test asserting `AssertCommandDrivenStateIsEmpty()` throws on residual state; plus the opted-out undo/redo-subject fixture -- covers all four matrix rows.

**Acceptance Criteria:**
- Given a scenario mutated fight, log and history, when its teardown runs, then every command is undone, the command-driven state asserts empty, leftover characters are deleted and the repository `Count` is 0.
- Given two scenarios run back to back, when the second starts, then it observes an empty fight, empty log and empty repository regardless of what the first did.
- Given a scenario overrides `UndoAllCommandsOnTeardown` to false, when its teardown runs, then the automatic undo loop and emptiness assertion are skipped and the scenario owns its own history cleanup.
- Given the full scenario suite, when it runs in a randomised order, then it passes.

## Design Notes

**Human renegotiation of the SPEC default.** The frozen SPEC states the production `CommandHistoryMaxSize` default is 64. During planning the human raised the production default to **256** (test host still 1000). Implement 256 as the new production default; the test-only value remains 1000.

**Opt-out semantics.** `UndoAllCommandsOnTeardown` (default `true`) gates the undo loop *and* its emptiness assertion — an undo/redo-subject scenario that deliberately ends with a populated history or redo stack overrides it to `false` and takes responsibility for leaving history clean. Character deletion is a separate, non-command-driven mechanism and is not tied to this flag.

**Mediator reachability.** `IUndoableMediator` is a host singleton (the library is singleton, not thread-safe), so the test process reaches the same history the browser circuit populated by resolving it from the running host's `IServiceProvider`. The same applies to `IFightContext`, `ICharacterRepository`, `ICombatTurnService`, `IAppliedStatusRepository` and `IDnDLogService`.

**Log teardown (human renegotiation).** The frozen SPEC listed `IDnDLogService.Blocks` among the surfaces to assert empty. Log undo is hide-based, not remove-based, so a full undo can never empty `Blocks`. Per the human: undo hides everything, so teardown asserts no visible entry remains (proving undo worked), then calls the existing `Clear()` for a genuinely empty slate. This mirrors the character-repository seam the SPEC already sanctions — state undo cannot reach is cleaned through an existing business API. `Clear()` pre-exists, so "no `Reset()` *added*" holds.

**Meta folder (human renegotiation).** Tests that validate the framework itself (the harness, the fixture, isolation) rather than the application live under `uitests/UiTestNavigation/Meta/` in the `DnDFightTool.UiTests.UiTestNavigation.Meta` namespace. Both the relocated `HomePageSmokeTests` and the new `ScenarioIsolationTests` sit there. `AssemblyFixture`'s `[SetUpFixture]` in the root `DnDFightTool.UiTests` namespace still applies to the sub-namespace.

**Redo-stack caveat.** After a full undo the redo stack still holds the undone commands (`RedoHistoryMaxSize` 32) — and nothing in the `IUndoableMediator` interface (verified against `2.0.0-alpha3`) clears it; "clean state" and "empty redo stack" are mutually exclusive through the public API. State is clean, but a scenario asserting on undo/redo button availability at its very first step would see a stale redo; the first new command clears it. Not a teardown concern, noted for scenario authors.

## Verification

**Commands:**
- `dotnet build DnDFightTool.slnx` -- expected: SUCCESS.
- `dotnet test uitests/UiTestNavigation/UiTestNavigation.csproj` -- expected: the smoke and isolation scenarios pass, including a randomised-order run.

## Suggested Review Order

**Scenario isolation and teardown (the core)**

- Entry point: the per-scenario teardown \u2014 undo everything, prove empty, delete characters.
  [`ApplicationFixture.cs:89`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L89)

- The reusable emptiness proof; note the log asserts no *visible* entry (undo is hide-based).
  [`ApplicationFixture.cs:124`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L124)

- Opt-out for undo/redo-subject scenarios; gates the undo loop and its assertion only.
  [`ApplicationFixture.cs:74`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L74)

**Test host composition**

- History ceiling raised to 1000 in the test host only.
  [`ApplicationFixture.cs:160`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L160)

- Optional param (production default 256), validated like `dataFolder`.
  [`ServiceCollectionExtensions.cs:44`](../../../../src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs#L44)

- Fed into the mediator's `CommandHistoryMaxSize`.
  [`ServiceCollectionExtensions.cs:72`](../../../../src/Components/DndUi.Web/IoC/ServiceCollectionExtensions.cs#L72)

**Emptiness surface**

- New global read surface so teardown can assert applied statuses are empty.
  [`IAppliedStatusRepository.cs:35`](../../../../src/Domain/Fight/IAppliedStatusRepository.cs#L35)

- Implementation returns the dictionary values.
  [`AppliedStatusRepository.cs:59`](../../../../src/Domain/Fight/AppliedStatusRepository.cs#L59)

**Scenarios (the proof)**

- Clean-slate, isolation and logging scenarios that rely on teardown.
  [`ScenarioIsolationTests.cs:26`](../../../../uitests/UiTestNavigation/Meta/ScenarioIsolationTests.cs#L26)

- BROKEN_UNDO safety property: the emptiness assertion trips on residual state.
  [`ScenarioIsolationTests.cs:89`](../../../../uitests/UiTestNavigation/Meta/ScenarioIsolationTests.cs#L89)

- Opted-out undo/redo-subject fixture owning its own cleanup.
  [`ScenarioIsolationTests.cs:113`](../../../../uitests/UiTestNavigation/Meta/ScenarioIsolationTests.cs#L113)

- Host-services accessor used by teardown and scenarios.
  [`ApplicationFixture.cs:60`](../../../../uitests/UiTestNavigation/ApplicationFixture.cs#L60)
