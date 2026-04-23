---
applyTo: "src/Business/DnDQueries/**/*.cs"
---

# Query Conventions (UndoableMediator)

- **Base class**: Queries inherit from `QueryBase<TResponse>` (from UndoableMediator). Handlers inherit from `QueryHandlerBase<TQuery, TResponse>`.
- **Handler constructor**: Query handlers do **not** receive `IUndoableMediator`. They only inject the services they need (e.g., `IDialogServiceProvider`).
- **Methods** (UndoableMediator v2.0.0-alpha1 API):
  - `ExecuteAsync(TQuery query)` → returns `Task<IQueryResponse<TResponse>>`
- **Read-only**: Queries must not mutate state. They are not added to undo history.
- **Naming**: `{Action}Query` and `{Action}QueryHandler`.
- **Location**: Query contracts live in `src/Business/DnDQueries/{Category}/`. Query handlers live in `src/UI/DnDQueryPrompter/` since they depend on UI (MudBlazor dialogs).
- **Base types**: `CasterTargetQueryBase<T>` provides `CasterId` and `TargetId` for queries involving two characters.
- **Dialog integration**: `IDialogServiceProvider` (defined in `DnDQueries`) bridges query handlers to MudBlazor dialogs for interactive queries (e.g., save roll prompts).
- **Responses**: Use `QueryResponse<T>.Success(value)` / `.Failed(value)` / `.Canceled(value)`.
