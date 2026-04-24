# Implementation Plan: DnD Log System

**Branch**: `001-fight-log-system` | **Date**: 2026-04-24 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-fight-log-system/spec.md`

## Summary

Build a structured game-event logging system (`IDnDLogService`) that command handlers use to produce tokenized, formatted log entries via write-log sub-commands through UndoableMediator. The system supports visual blocks, nested indentation scopes, BBCode-like formatting (bold, semantic colors, hover tooltips), undo/redo visibility integration, and a Blazor log component replacing the fight page placeholder. All 9 existing command handlers will be retrofitted to produce log entries, and a developer skill file will be created last.

## Technical Context

**Language/Version**: C# 13 / .NET 10 (`net10.0`)
**Primary Dependencies**: UndoableMediator, MudBlazor v8.x, FluentValidation, Mapster
**Storage**: In-memory (singleton, session-scoped)
**Testing**: NUnit 4 + FluentAssertions 7 + FakeItEasy 9
**Target Platform**: .NET MAUI Hybrid (Windows-first), Blazor
**Project Type**: Desktop app (MAUI Hybrid)
**Performance Goals**: N/A — in-memory list, no hot path
**Constraints**: No new dependencies without justification
**Scale/Scope**: ~200 log entries per fight session (typical), unbounded max

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate | Status | Notes |
|------|--------|-------|
| No new dependencies | PASS | No new NuGet packages needed. BBCode parser is hand-rolled (simple regex/string parsing). |
| File-scoped namespaces | PASS | All new files will use file-scoped namespaces. |
| UndoableMediator for all mutations | PASS | Write-log is a sub-command dispatched via `SendAsSubCommandAsync`. |
| Domain has no external deps | PASS | `IDnDLogService` interface lives in Domain or a new Domain-adjacent project. LogEntry/LogBlock are pure domain types. |
| Layer dependency direction | PASS | Domain ← Business ← UI. Service interface in Domain; implementation registered in composition root. |
| No logic in .razor files | PASS | All log component C# in code-behind `.razor.cs`. |
| DI via ServiceCollectionExtensions | PASS | New project exposes `Register*Services()`. |
| Tests for command handlers | PASS | New tests for WriteLogCommandHandler + updates to existing handler tests. |
| Blazor components inherit StylableComponentBase | PASS | Log component will inherit StylableComponentBase. |

**No violations. Gate passed.**

## Project Structure

### Documentation (this feature)

```text
specs/001-fight-log-system/
├── plan.md              # This file
├── spec.md              # Feature specification
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (speckit.tasks command)
```

### Source Code (repository root)

```text
src/
├── Domain/
│   └── Logs/                                # NEW project: Logs.csproj
│       ├── IDnDLogService.cs                # Service interface + change event
│       ├── LogEntry.cs                      # LogEntry entity
│       ├── LogBlock.cs                      # LogBlock entity
│       ├── LogColorToken.cs                 # Enum: supported color tokens
│       ├── DnDLogService.cs                 # Singleton implementation
│       ├── Logs.csproj
│       └── IoC/
│           └── ServiceCollectionExtensions.cs
│
├── Business/
│   └── DnDActions/
│       └── LogActions/                      # NEW folder
│           ├── WriteLog/
│           │   ├── WriteLogCommand.cs       # Sub-command: writes a LogEntry
│           │   └── WriteLogCommandHandler.cs
│           ├── OpenBlock/
│           │   ├── OpenBlockCommand.cs      # Sub-command: opens a named log block
│           │   └── OpenBlockCommandHandler.cs
│           ├── CloseBlock/
│           │   ├── CloseBlockCommand.cs     # Sub-command: closes the current log block
│           │   └── CloseBlockCommandHandler.cs
│           ├── OpenScope/
│           │   ├── OpenScopeCommand.cs      # Sub-command: increments indent level
│           │   └── OpenScopeCommandHandler.cs
│           ├── CloseScope/
│           │   ├── CloseScopeCommand.cs     # Sub-command: decrements indent level
│           │   └── CloseScopeCommandHandler.cs
│           └── DamageTypeLogExtensions.cs   # Extension: DamageTypeEnum → color token string
│
├── UI/
│   └── FightBlazorComponents/
│       └── Log/                             # NEW folder
│           ├── DnDLogComponent.razor        # Main log panel component
│           ├── DnDLogComponent.razor.cs     # Code-behind
│           ├── DnDLogComponent.razor.css    # Scoped CSS (block spacing, hover)
│           ├── LogBlockComponent.razor      # Single block renderer
│           ├── LogBlockComponent.razor.cs
│           ├── LogBlockComponent.razor.css
│           ├── LogEntryComponent.razor      # Single entry renderer (BBCode→HTML)
│           ├── LogEntryComponent.razor.cs
│           ├── LogEntryComponent.razor.css
│           └── Parsing/
│               ├── LogTokenParser.cs        # BBCode tag tokenizer
│               └── LogToken.cs              # Token types (Text, Bold, Color, Hover)
│
├── Components/
│   └── DndUi.Shared/
│       └── wwwroot/
│           └── css/
│               └── log-colors.css           # NEW: --log-color-{token} CSS variables

tests/
├── Domain/
│   └── LogsTests/                           # NEW test project
│       ├── DnDLogServiceTests.cs
│       ├── LogEntryTests.cs
│       └── LogBlockTests.cs
├── Business/
│   └── DnDActionsTests/
│       └── LogActions/
│           └── WriteLogCommandHandlerTests.cs  # NEW
└── UI/
    └── FightBlazorComponentsTests/          # NEW test project (if needed)
        └── Log/
            └── LogTokenParserTests.cs       # NEW
```

**Structure Decision**: New `Logs` project under `src/Domain/` for the service interface, entities, enum, and implementation. This keeps the log system independent of `Fight` (since logs are not fight-scoped per clarification). Named `Logs` (not `DnDLog`) because the `DnD` prefix is redundant within this application.

The write-log and structural sub-commands live in `DnDActions/LogActions/`. Block/scope lifecycle (open, close) is managed via dedicated sub-commands (`OpenBlockCommand`, `CloseBlockCommand`, `OpenScopeCommand`, `CloseScopeCommand`) so that the entire log lifecycle is in the undo tree and no business handler needs to inject `IDnDLogService` directly. `IDnDLogService` is only injected by the five leaf handlers in `LogActions/`. Undo of structural commands is a no-op: `DnDLogComponent` already skips blocks with no visible entries, so hiding all `WriteLogCommand` entries makes a block disappear automatically.

The Blazor components live in `FightBlazorComponents/Log/` since the log panel is on the fight page. The BBCode parser is collocated with the components since it's a presentation concern.

## Complexity Tracking

No constitution violations — this table is intentionally empty.
