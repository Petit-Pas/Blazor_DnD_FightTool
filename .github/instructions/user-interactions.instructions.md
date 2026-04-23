---
applyTo: "src/UI/DnDQueryPrompter/**/*.cs"
---

# Query Prompting Conventions (`DnDQueryPrompter`)

Interactive queries bridge business logic with Blazor UI via dialog-based query handlers. The `DnDQueryPrompter` project (`src/UI/DnDQueryPrompter/`) is the UI-layer project that implements these handlers.

## 1. Dialog Service

- `IDialogServiceProvider` — interface defined in `DnDQueries`. Exposes `SetDialogService(IDialogService)` / `GetDialogService()`.
- `DialogServiceProvider` — singleton implementation in `DnDQueryPrompter`. Registered via `RegisterDnDQueryPrompterServices()` (or equivalent IoC method).
- The host page (e.g., `FightPage`) must call `DialogServiceProvider.SetDialogService(DialogService)` in `OnInitializedAsync` before any query handler runs.

## 2. Query Handlers

- Inherit `QueryHandlerBase<TQuery, TResponse>` (from UndoableMediator).
- Inject `IDialogServiceProvider` via constructor (no `IUndoableMediator` — queries are read-only).
- Show dialogs via `_dialogServiceProvider.GetDialogService().ShowAsync<TDialog>(title, parameters, options)`, then `await dialog.Result`.
- Named `{Action}QueryHandler`. Located in `{Category}/` subfolders.
- Return `QueryResponse<T>.Success(value)` on confirmation, `.Canceled(value)` when `dialog.Result.Canceled`.

## 3. Modal Components

- Razor component + code-behind pairs (e.g., `MartialAttackRollResultQueryHandlerModal`).
- Receive domain data via `[Parameter]` (set via `DialogParameters<TModal>`).
- Use MudBlazor + `DnDFightTool.UI.SharedComponents` for rendering.
- Named `{Action}Modal` or `{Action}QueryHandlerModal`.

## Example

```csharp
// Query handler
public class SaveRollResultQueryHandler : QueryHandlerBase<SaveRollResultQuery, SaveRollResult>
{
    private readonly IDialogServiceProvider _dialogServiceProvider;

    public SaveRollResultQueryHandler(IDialogServiceProvider dialogServiceProvider)
    {
        _dialogServiceProvider = dialogServiceProvider;
    }

    public override async Task<IQueryResponse<SaveRollResult>> ExecuteAsync(SaveRollResultQuery query)
    {
        var parameters = new DialogParameters<SaveRollResultQueryHandlerModal>
        {
            { x => x.SaveRollResult, query.SaveRollResult }
        };
        var dialog = await _dialogServiceProvider.GetDialogService()
            .ShowAsync<SaveRollResultQueryHandlerModal>("Save Roll", parameters);
        var result = await dialog.Result;

        return result?.Canceled ?? true
            ? QueryResponse<SaveRollResult>.Canceled(query.SaveRollResult)
            : QueryResponse<SaveRollResult>.Success(query.SaveRollResult);
    }
}
```
