# Implementation Plan: Undo/Redo Buttons

**Branch**: `003-undo-redo-buttons` | **Date**: 2026-04-27 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/003-undo-redo-buttons/spec.md`

## Summary

Add undo and redo icon buttons to the existing `CombatStatusComponent` (bottom-right panel of the fight page). The buttons call `IUndoableMediator.UndoLastCommandAsync()` / `RedoLastUndoneCommandAsync()` and grey out based on `HistoryLength` / `RedoHistoryLength`. State reactivity is driven by subscribing to three new events (`OnCommandExecuted`, `OnCommandUndone`, `OnCommandRedone`) that will be added to the UndoableMediator NuGet package as a prerequisite update. No new domain types, commands, or projects are needed.

## Technical Context

**Language/Version**: C# 14 / .NET 10 (`net10.0`)  
**Primary Dependencies**: UndoableMediator (update from `2.0.0-alpha2` → `2.0.0-alpha3`+), MudBlazor v8.x  
**Storage**: N/A (no persistence; derived from in-memory mediator state)  
**Testing**: NUnit 4 + FluentAssertions 7 + FakeItEasy 9 (bUnit for component tests if applicable)  
**Target Platform**: .NET MAUI Hybrid (Windows-first), Blazor  
**Project Type**: Desktop app (MAUI Hybrid)  
**Performance Goals**: N/A — button clicks, no hot path  
**Constraints**: No new NuGet dependencies (only a version bump of an existing one)  
**Scale/Scope**: Two buttons in one existing component

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate | Status | Notes |
|---|---|---|
| No new dependencies | PASS | Only a version update of the already-referenced UndoableMediator package. |
| File-scoped namespaces | PASS | No new files require namespaces; existing file already uses file-scoped. |
| UndoableMediator for all mutations | PASS | Undo/Redo are mediator operations — no direct state mutation. |
| Domain has no external deps | PASS | No domain changes. |
| Layer dependency direction | PASS | UI → Business → Domain. No reverse flow introduced. |
| No logic in .razor files | PASS | All C# stays in `.razor.cs` code-behind. |
| DI via ServiceCollectionExtensions | PASS | No new DI registrations needed — `IUndoableMediator` is already registered. |
| Tests for command handlers | N/A | No new command handlers. Component tests may be added. |
| Blazor components inherit StylableComponentBase | PASS | `CombatStatusComponent` already inherits `StylableComponentBase`. |
| IFightContext is source of truth for fight session | PASS | No fight state changes; buttons delegate to mediator. |

**No violations. Gate passed.**

**Post-design constitution re-check**: No additional violations. The only external change is a NuGet version bump of an existing package — no new dependency.

## Project Structure

### Documentation (this feature)

```text
specs/003-undo-redo-buttons/
├── plan.md              # This file
├── spec.md              # Feature specification
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (speckit.tasks command — NOT created here)
```

### Source Code Changes

```text
src/
├── UI/
│   └── FightBlazorComponents/
│       └── CombatStatus/
│           ├── CombatStatusComponent.razor            # MODIFIED — add undo/redo buttons
│           ├── CombatStatusComponent.razor.cs         # MODIFIED — add event subscriptions + handlers
│           └── CombatStatusComponent.razor.css        # MODIFIED — add button row styling
│
├── Business/
│   └── DnDActions/
│       └── DnDActions.csproj                          # MODIFIED — UndoableMediator version bump
│
│   └── DnDQueries/
│       └── DnDQueries.csproj                          # MODIFIED — UndoableMediator version bump
│
├── Infrastructure/
│   └── Extensions/
│       └── Extensions.csproj                          # MODIFIED — UndoableMediator version bump
│
└── Components/
    ├── DndUi/
    │   └── DndUi.csproj                               # MODIFIED — UndoableMediator version bump
    └── DndUi.Web/
        └── DndUi.Web.csproj                           # MODIFIED — UndoableMediator version bump

tests/
└── UI/
    └── FightBlazorComponentsTests/
        └── CombatStatus/
            └── CombatStatusComponentTests.cs          # NEW (optional) — bUnit tests
```

**Structure decision**: No new projects. All changes are modifications to existing files (one component, five `.csproj` version bumps). Optional bUnit test class may be added.

## Complexity Tracking

No constitution violations — this table is intentionally empty.

## Key Design Decisions

### D-001: Buttons embedded in CombatStatusComponent (not a separate component)

The undo/redo buttons are added directly to the existing `CombatStatusComponent` rather than extracted into a standalone component. Rationale: only two buttons, tightly coupled to panel layout, and the spec explicitly says "within the component that displays rounds and turns." If complexity grows, they can be extracted later.

### D-002: Event-driven reactivity via IUndoableMediator events

The component subscribes to `OnCommandExecuted`, `OnCommandUndone`, and `OnCommandRedone`. Each event triggers `InvokeAsync(StateHasChanged)`. Button disabled state is recomputed on render from `HistoryLength` / `RedoHistoryLength`. This is the same pattern used for `ICombatTurnService.OnChanged`.

### D-003: Library prerequisite — UndoableMediator version bump

The feature is blocked until the UndoableMediator package is updated to expose the three events. This is documented as a prerequisite in `quickstart.md`. The implementation task list should start with the NuGet update.

### D-004: No confirmation dialogs, no keyboard shortcuts

Per the spec: undo/redo is immediate (no confirmation). Keyboard shortcuts are explicitly out of scope.

### D-005: MudIconButton with Material Undo/Redo icons

Using `MudIconButton` with `Icons.Material.Filled.Undo` / `Icons.Material.Filled.Redo`. The `Disabled` parameter provides built-in greyed-out styling — no custom CSS needed for the disabled state.

## Prerequisite

> **BLOCKING**: The UndoableMediator NuGet package must be updated to expose `OnCommandExecuted`, `OnCommandUndone`, and `OnCommandRedone` events on `IUndoableMediator` before implementation can begin. Publish as `2.0.0-alpha3` (or later) and update all project references.
