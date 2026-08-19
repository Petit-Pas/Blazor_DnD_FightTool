---
project_name: 'DnDFightTool'
user_name: 'Benoit'
date: '2026-05-29'
sections_completed:
  ['technology_stack', 'csharp_rules', 'blazor_rules', 'command_handler', 'testing', 'code_organization', 'critical_rules']
status: 'complete'
rule_count: 42
optimized_for_llm: true
---

# Project Context for AI Agents

_This file contains critical rules and patterns that AI agents must follow when implementing code in this project. Focus on unobvious details that agents might otherwise miss._

---

## Technology Stack & Versions

- **.NET 10.0** (C# 14) — SDK 10.0.100, `rollForward: latestFeature`
- **Blazor Hybrid** — dual-host: MAUI (`DndUi`) + ASP.NET Core (`DndUi.Web`), shared via `DndUi.Shared` Razor class lib. MAUI is a thin shell; treat all UI as standard Blazor.
- **MudBlazor 9.4.0** — all UI uses MudBlazor components. Never use raw HTML when a MudBlazor equivalent exists. v9 has breaking changes from v7/v8 — do not generate older API patterns.
- **UndoableMediator 2.0.0-alpha3** — custom CQRS mediator with undo/redo. Do NOT look this up externally; follow `.github/skills/undoable-mediator/SKILL.md`. Never use MediatR.
- **Mapster 10.0.7** — wrapped in custom `IMapper`. Never call `TypeAdapterConfig` or `.Adapt<T>()` directly. Use `IMapper.Clone<T>()` (new IDs, duplication) or `IMapper.Copy<T>()` (same IDs, edit snapshot).
- **FluentValidation 12.1.1** — always inherit `PropertyTargetedValidator<T>`, never `AbstractValidator<T>`. This base provides `ValidateValue` for MudBlazor per-field validation.
- **NUnit 4.5.1 + FluentAssertions 7.2.0 + FakeItEasy 9.0.1** — always use strict fakes: `A.Fake<T>(options => options.Strict())`.

### Dependency Interactions

- `IMapper.Copy<T>()` (same IDs) is used in Blazor components to snapshot an entity before editing, enabling local undo/cancel of edits. `IMapper.Clone<T>()` (new IDs) is for duplication.
- Dialog queries go through `IDialogServiceProvider` bridge, not raw MudBlazor `IDialogService`.
- Code in `DndUi.Shared` runs in both MAUI and Web — no platform-specific APIs.

## Critical Implementation Rules

### C# Language Rules

**Banned patterns (agents will attempt these — reject immediately):**
- ❌ **Primary constructors** — never `class Foo(IBar bar)`. Use traditional constructors with `_camelCase` fields.
- ❌ **Expression-bodied members** — never `=>` for methods, properties, or accessors. Always block bodies `{ }`.
- ❌ **`AbstractValidator<T>`** — always inherit `PropertyTargetedValidator<T>` (custom base for Blazor per-field validation).

**Required patterns:**
- File-scoped namespaces only.
- `record` for immutable value objects, `class` for mutable entities with identity.
- Collection expressions `[]` for empty/initialized collections.
- Async/await always — never `.Result` or `.GetAwaiter().GetResult()`.
- XML doc comments (`/// <summary>`) on public and internal members.
- `[Obsolete]` on parameterless constructors (serialization-only); use `bool withDefaults` constructor for production code.
- Interface-first for domain entities (`ICharacter` → `Character`).
- Enums get companion `{Enum}Extensions` class with static `All` array.
- Naming: PascalCase types/methods/properties, `_camelCase` private fields, `I`-prefix interfaces.

### Blazor & MudBlazor Rules

**Component architecture:**
- `.razor` is markup-only — ALL logic in `.razor.cs` code-behind (partial class). No `@code { }` blocks.
- Code-behind: `public partial class ComponentName : ComponentBase` (or `: StylableComponentBase` when component accepts `Class`/`Style` pass-through).
- Services injected via `[Inject] public required IService Service { get; set; }`.
- Parameters via `[Parameter]`, cascading via `[CascadingParameter]`.
- Implement `IDisposable` — subscribe to events in `OnInitialized()`, unsubscribe in `Dispose()`.

**Banned Blazor patterns:**
- ❌ Raw HTML elements when MudBlazor equivalent exists (`<input>` → `<MudTextField>`, `<button>` → `<MudButton>`, etc.)
- ❌ Injecting `IDialogService` in components/handlers — use `IDialogServiceProvider` bridge. Exception: host pages that are responsible for feeding `IDialogServiceProvider` may inject `IDialogService` to call `SetDialogService()`.
- ❌ Bare `StateHasChanged()` — always `InvokeAsync(StateHasChanged)` for thread safety.
- ❌ Computed CSS properties in code-behind — use `"css-class".When(condition)` inline in markup.

**State & rendering:**
- `IFightContext` singleton holds fight state; components subscribe to `OnFighterUpdated` event.
- `IGlobalEditContext` for cross-navigation entity state management.
- `IMapper.Copy<T>()` to snapshot an entity before editing (enables cancel/undo of local edits in a component).

### Command/Handler Architecture

**Orchestrator vs Atomic pattern (project-specific — no external training data covers this):**
- **Orchestrators** (`{Action}Command` / `{Action}CommandHandler`) — coordinate sub-commands; NEVER mutate state directly.
- **Atomics** (`{Action}AtomicCommand` / `{Action}AtomicCommandHandler`) — leaf mutations; suffix `Atomic` marks internal-only commands.
- Each command pair lives in its own folder under `src/Business/DnDActions/`.
- Base classes: `TargetCommandBase`, `CasterCommandBase`, `CasterTargetCommandBase` (all extend `CommandBase`).
- Handlers extend `CommandHandlerBase<TCommand>`.

**Banned command patterns:**
- ❌ **Injecting `IDnDLogService`** — logging via `WriteLogCommand`/`OpenBlockCommand`/`CloseBlockCommand` sub-commands only. This is the #1 rule agents will break.
- ❌ **Mutation in orchestrator handlers** — orchestrators only dispatch sub-commands.
- ❌ **Skipping `NotifyFighterUpdated()`** — always call `_fightContext.NotifyFighterUpdated()` after state mutation.

**Undo/Redo mechanics:**
- Orchestrator `Undo` → `base.UndoAsync(command)` (cascades to sub-commands).
- Orchestrator `Redo` (default) → `base.RedoAsync(command)` (replays existing sub-commands, preserving captured state like dice rolls).
- Orchestrator `Redo` (state-dependent) → check if relevant state has changed since execution; if so, `ClearSubCommands(command)` then re-call `ExecuteAsync(command)`. Use this when the command outcome depends on state that could be different (e.g., attack template modified, or hit/miss outcome would change). See `ExecuteMartialAttackCommandHandler` for the reference pattern.
- Atomic `Undo` → direct state reversal, returns `Task.CompletedTask`.
- Log entries are sub-commands, automatically undone/redone with the parent.

**Query pattern:**
- Query contracts in `DnDQueries`, handlers in `DnDQueryPrompter` (UI layer) — this split is intentional.
- Queries extend `QueryBase<T>`, handlers extend `QueryHandlerBase<TQuery, TResponse>`.
- No `IUndoableMediator` in query handlers — only domain services + `IDialogServiceProvider`.
- Return `QueryResponse<T>.Success` / `.Failed` / `.Canceled`.

### Testing Rules

**Strict fakes always:**
- `A.Fake<T>(options => options.Strict())` — non-configured calls throw.
- Mediator: `A.Fake<IUndoableMediator>(o => o.Strict().Implements<ISubCommandDispatcher>())`.

**Test structure:**
- Nested fixtures: `ExecuteTests : ParentTests`, `UndoTests : ParentTests`, `RedoTests : ParentTests`.
- AAA pattern with `// Arrange`, `// Act`, `// Assert` comments.
- Method naming: `Should_{ExpectedBehavior}` or `Should_{Behavior}_When_{Condition}`.
- Fields initialized as `= null!`, assigned in `[SetUp]`.

**Test utilities:**
- `DomainTestsUtilities` project (shared, not a test project) contains factories.
- `CharacterFactory.BuildMonster()` / `.BuildPlayer()` — never `new Character()`.
- Test namespace mirrors source but without `DnDFightTool.` prefix.

### Code Organization & Infrastructure

**IoC registration:**
- Each project: `IoC/ServiceCollectionExtensions.cs` with `Register{Feature}Services(this IServiceCollection)`.
- Singleton: `IFightContext`, `IMapper`, `ICharacterRepository`, `IJsonSerializer`.
- Scoped: `IDiceRollNotifier`. 
- Transient: validators.
- UndoableMediator: `ConfigureMediator()` scans `DnDActions` + `DnDQueryPrompter` assemblies.

**Mapping:**
- `IMapper.Clone<T>()` = new IDs (duplication). `IMapper.Copy<T>()` = same IDs (edit snapshot — used in Blazor components before opening an editor, so the original can be restored on cancel). Mixing breaks identity or loses the original.
- **Monster vs Player in a fight**: monsters are templates — they are `Clone`d (new IDs) when added to a fight so the original template is never mutated. Players are added by reference — they always keep their original ID.
- Dictionary-keyed-by-Id collections need `AfterMapping` to rebuild.
- Per-feature `MappingConfigurations.cs` as `internal static class`.
- `IgnoreWhenDuplicating(x => x.Id)` for ID fields.

**Domain entities:**
- Interface-first (`ICharacter` → `Character`).
- Validation in `Validation/` subfolder, mapping in `Mapping/` subfolder.
- `IHashable` marker interface for change-tracking.
- `InternalsVisibleTo` for test projects.

### Critical Don't-Miss Rules (Highest Agent Violation Rate)

1. **Never inject `IDnDLogService`** — use `WriteLogCommand` sub-commands for logging in handlers.
2. **Never use primary constructors** — agents will attempt this on every new class.
3. **Never use `AbstractValidator<T>`** — always `PropertyTargetedValidator<T>`.
4. **Orchestrator handlers MUST NOT mutate state** — delegate to atomic sub-commands only.
5. **`Copy<T>()` for edit snapshots (same IDs), `Clone<T>()` for duplication (new IDs)** — semantic difference agents won't guess; `Copy` is used in components before editing, not in command handlers.
6. **Query handlers live in `DnDQueryPrompter` (UI layer)**, not `DnDQueries` — counterintuitive but intentional.
7. **Always strict fakes** with `Implements<ISubCommandDispatcher>()` for mediator.
8. **Read `.github/skills/undoable-mediator/SKILL.md`** before implementing any command/handler — external docs don't cover this library.
9. **Read `.github/skills/dnd-logging/SKILL.md`** before implementing log entries — BBCode formatting, color tokens, block/scope structure.

---

## Two Planning Systems Coexist — Shared Feature Numbering

Features may be defined by **either** system. Both are valid; neither owns the repo.

| System | Location | Kernel file |
|---|---|---|
| Spec Kit | `specs/NNN-kebab-name/` | `spec.md` + `plan.md`, `tasks.md`, … |
| BMAD | `_bmad-output/specs/spec-NNN-kebab-name/` | `SPEC.md` + companions |

**`NNN` is one shared sequence across both systems.** Before creating a feature in
either, scan **both** folders for the highest `NNN`, then use highest+1 zero-padded to
3 digits. Never reuse a number, even one owned by the other system.

- Highest currently in use: **004** — next feature is **005**, whichever system creates it.
- Acknowledge both locations when orienting; read whichever holds the feature in question.
- Do **not** migrate, duplicate, or mirror a feature between systems.
- Do **not** assume the system you were invoked from owns the active work — check both.

> Note the name collision: `specs/` at repo root is Spec Kit; `_bmad-output/specs/` is
> BMAD. Different trees, same word — always qualify which one you mean.

---

## Usage Guidelines

**For AI Agents:**
- Read this file before implementing any code.
- Follow ALL rules exactly as documented.
- When in doubt, prefer the more restrictive option.
- Read referenced skill files (`.github/skills/`) for detailed API guidance.

**For Humans:**
- Keep this file lean and focused on agent needs.
- Update when technology stack or patterns change.
- Remove rules that become obvious over time.

Last Updated: 2026-05-29
