# Project Constitution — DnDFightTool

Non-negotiable principles for this project. Every spec, plan, task, and agent response must comply with these rules before anything else.

---

## 1. Technology Stack

| Concern | Choice |
|---|---|
| Runtime | .NET 10 (`net10.0`), SDK 10.0.100 |
| Language | C# 13 — `ImplicitUsings`, `Nullable` enabled everywhere |
| Host | .NET MAUI Hybrid (Windows-first). MAUI provides only local/device services. Treat all UI as standard Blazor unless MAUI platform APIs are explicitly needed. |
| UI framework | Blazor + **MudBlazor v8.x**. Never use raw HTML form controls where a MudBlazor equivalent exists. |
| Mediator | **UndoableMediator** — all commands and queries go through `IUndoableMediator`. No direct service calls for business operations. |
| Mapping | **Mapster** via the custom `IMapper` / `Mapper` wrapper in `src/Infrastructure/Mapping/`. |
| Validation | **FluentValidation** — validators are separate classes, never inline in entities. |
| DI container | `Microsoft.Extensions.DependencyInjection` — wired in `DndUi/MauiProgram.cs`. |
| Tests | **NUnit 4**, **FluentAssertions 7**, **FakeItEasy 9** |

No new dependencies may be introduced without explicit justification and confirmation.

---

## 2. Architecture Layers & Projects

The solution is organized into strict layers. Dependencies flow **inward only** (outer layers depend on inner layers, never the reverse).

```
src/Components/                              ← Host & composition root layer
    ├── DndUi                                ← MAUI host. Composition root (MauiProgram.cs).
    │                                          Registers all DI, wires all layers.
    │                                          Contains UserInteractionHandlers/.
    ├── DndUi.Shared                         ← Shared Blazor layout & pages used by
    │                                          both MAUI and Web hosts. References
    │                                          UI projects + MudBlazor.
    └── DndUi.Web                            ← Web host (placeholder, empty for now).

src/UI/                                      ← Blazor components (presentation)
    ├── SharedComponents                     ← Generic, D&D-agnostic reusable components
    │                                          (Buttons, Containers, Dialogs, Icons, Inputs).
    │                                          Also hosts StylableComponentBase.
    ├── DnDEntitiesBlazorComponents          ← Character sheet editor: components for
    │                                          creating and editing D&D domain entities
    │                                          (ability scores, attacks, saves…).
    │                                          Hosts GlobalEditContext for edit state.
    ├── FightBlazorComponents                ← Live fight screen components:
    │                                          FightingCharacter cards, MartialAttack
    │                                          panels, initiative order, etc.
    ├── DnDQueryPrompter                     ← Query handlers + dialog bridge.
    │                                          Contains DialogServiceProvider,
    │                                          IDialogServiceProvider, and query handlers
    │                                          (e.g., SaveRollResultQueryHandler).
    │                                          Mirrors DnDQueries folder structure.
    └── JavascriptInteropExtensions          ← JS interop utilities (e.g., scroll helpers)

src/Business/                                ← Use cases / application logic
    ├── DnDActions                           ← Commands (mutate state, undo/redo)
    ├── DnDQueries                           ← Query definitions (read-only, no undo).
    │                                          Query handlers live in DnDQueryPrompter.
    └── DnDUserInterations                   ← User interaction contracts & service

src/Domain/                                  ← Core domain model (no external deps)
    ├── DnDEntities                          ← Reusable D&D entities (characters, attacks, etc.)
    └── Fight                                ← Fight-session state (FightContext, FightingCharacter)

src/Infrastructure/                          ← Cross-cutting utilities
    ├── Extensions                           ← Enum helpers, general extensions
    ├── Mapping                              ← Mapster IMapper wrapper + MappingConfigurations
    ├── Memory                               ← IHashable, change-tracking
    ├── IO                                   ← File I/O, serialization
    └── AspNetCoreExtensions                 ← ASP.NET-specific helpers (if needed)
```

### Layer rules

- `Domain` projects have **no dependency on Business, UI, or Infrastructure**.
- `Business` projects may depend on `Domain` and `Infrastructure` only.
- `UI` projects may depend on `Business`, `Domain`, and `Infrastructure`.
- `DndUi.Shared` hosts shared layout and pages; it references `UI` projects and `MudBlazor`.
- `DndUi` (MAUI composition root) references `DndUi.Shared` and UI projects. It is the only project that wires all DI registrations.
- `DndUi.Web` is a future web host — references `DndUi.Shared` when needed.
- `FightBlazorComponents` lives under `src/UI/FightBlazorComponents/`.

---

## 3. Where to Create New Files

| What you're creating | Where it goes |
|---|---|
| New D&D entity (character, spell, item…) | `src/Domain/DnDEntities/{Feature}/` |
| New fight-specific type or extension | `src/Domain/Fight/{Feature}/` |
| New enum | Same folder as related entity; companion `{Enum}Extensions` class in the same file or adjacent file |
| New command (mutates state) | `src/Business/DnDActions/{DnDEntity}Actions/{ActionName}/` — one folder per command+handler pair. `{DnDEntity}` matches a subfolder name from `src/Domain/DnDEntities/` (e.g., `Damage`, `HitPoint`, `MartialAttacks`, `Statuses`). |
| New query definition (read-only) | `src/Business/DnDQueries/{DnDEntity}/` — query class only, no handler here |
| New query handler | `src/UI/DnDQueryPrompter/{DnDEntity}Queries/` — mirrors `DnDQueries` structure |
| New query interaction modal | `src/UI/DnDQueryPrompter/{DnDEntity}Queries/` — next to the handler |
| New Blazor component for D&D entities (editor/sheet) | `src/UI/DnDEntitiesBlazorComponents/DnDEntities/{Feature}/` |
| New fight-screen Blazor component | `src/UI/FightBlazorComponents/Entities/{Feature}/` |
| New shared/generic Blazor component | `src/UI/SharedComponents/` |
| New JS interop helper | `src/UI/JavascriptInteropExtensions/` |
| New mapping configuration | `{Feature}/Mapping/MappingConfigurations.cs` next to the entities |
| New validator | `{Feature}/Validation/{Entity}Validator.cs` next to the entities |
| New infrastructure utility / extension | `src/Infrastructure/Extensions/` |
| DI registration for a new project | `{Project}/IoC/ServiceCollectionExtensions.cs` |
| New shared layout / page (used by both MAUI and Web) | `src/Components/DndUi.Shared/Components/` |
| New MAUI platform handler | `src/Components/DndUi/UserInteractionHandlers/` |
| New test project | `tests/{Layer}/{ProjectName}Tests/` |
| New test utility (factory, fake, extension) | `tests/Domain/DomainTestsUtilities/{Factories|Fakes|Extensions}/` |

---

## 4. Coding Conventions (Non-Negotiable)

- **File-scoped namespaces** everywhere — no block-scoped `namespace {}`.
- **Namespace roots**:
  - Domain → `DnDFightTool.Domain.{ProjectName}`
  - Business → `DnDFightTool.Business.{ProjectName}`
  - Infrastructure → short names (`Extensions`, `Mapping`, `Memory`, `IO`)
  - UI → short names (`SharedComponents`, `DnDEntitiesBlazorComponents`, `DnDQueryPrompter`)
- **Private fields**: `_camelCase`. Types: `PascalCase`. Interfaces: `I` prefix.
- **Nullable reference types** enabled. Use `?` explicitly; `!` only when certain.
- **No logic in `.razor` files** — all C# goes in the `.razor.cs` code-behind partial class.
- **Blazor components** that expose `Class` / `Style` parameters **must** inherit `StylableComponentBase`.

---

## 5. Command & Query Rules (UndoableMediator)

- Every state mutation goes through a **Command** (`ICommand` → `IUndoableMediator.SendAsync`).
- Every read goes through a **Query** (`IQuery<T>` → `IUndoableMediator.QueryAsync`).
- Queries **never mutate state**.
- Commands store old state during `ExecuteAsync` so `UndoAsync` can restore it exactly.
- Sub-commands are registered via `SendAsSubCommandAsync` — undoing a parent cascades automatically.
- `Redo` must call `base.RedoAsync` **after** re-applying state (or clear sub-commands and re-execute when the model may have changed). The class ExecuteMartialAttackCommandHandler is a good example of how complex Redo can become for commands that do a lot.
- Command handler constructors always take `IUndoableMediator mediator` as the first argument and call `base(mediator)`.

---

## 6. Dependency Injection Rules

- Every project exposes a single `static ServiceCollectionExtensions` in its `IoC/` folder.
- Method signature: `Register{Feature}Services(this IServiceCollection services) : IServiceCollection`.
- Lifetimes:
  - `Singleton`: `IFightContext`, `ICharacterRepository`, `IMapper`, `IUserInteractionService`, `IJsonSerializer`
  - `Scoped`: per-navigation/per-page services
  - `Transient`: validators
- `MauiProgram.cs` is the only composition root — it chains all `Register*` extension methods.

---

## 7. Testing Rules

- Every command handler gets a test class in `tests/Business/DnDActionsTests/`.
- Test class names: `{ClassUnderTest}Tests`. Nested inner classes for `ExecuteTests`, `UndoTests`, `RedoTests`.
- Assertion library: **FluentAssertions only** — no `Assert.That`, no `Assert.AreEqual`.
- Mocking: **FakeItEasy only** — no `Moq`, no `NSubstitute`.
- Test method naming: `Should_{ExpectedBehavior}` or `Should_{ExpectedBehavior}_When_{Condition}`.
- Use `DomainTestsUtilities` factories/fakes instead of constructing entities inline in tests.

---

## 8. Fight Session Model

- `IFightContext` is the single source of truth for the active fight session — it is a `Singleton`.
- `FightingCharacter` wraps a `Character` by **composition** (never inheritance).
- Monsters are **cloned** when added to a fight so the original template is never mutated.
- Players are added **by reference**.
- Fight-specific extension methods on domain types live in `src/Domain/Fight/DomainExtensions/`.

---

## 9. Query Handler Pattern

Query definitions (`QueryBase<T>` subclasses) live in `src/Business/DnDQueries/`. Their **handlers** live in `src/UI/DnDQueryPrompter/`, collocated with the dialog service provider. The folder structure in `DnDQueryPrompter` mirrors `DnDQueries` (e.g., `SaveQueries/`, `MartialAttackQueries/`).

- `IDialogServiceProvider` / `DialogServiceProvider` bridge query handlers to MudBlazor dialogs.
- Modal components live in the same `{DnDEntity}Queries/` folder as their handler.
- Never call MudBlazor `IDialogService` directly from a command or query handler — go through `IDialogServiceProvider`.

---

## 10. What This Project Is NOT

- **Not a web app**: There is no ASP.NET Core pipeline, no HTTP API, no authentication, no multi-user server.
- **Not a game engine**: This is a combat management tool — it does not simulate full D&D rules automatically.
- **Not persistence-heavy**: Persistence is in-memory by default. File I/O (JSON) is a secondary concern.
- **Not cross-platform at UI level**: The primary target is Windows. iOS/Android/Mac are secondary and must not drive architectural decisions.
