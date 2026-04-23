---
applyTo: "src/Business/DnDUserInterations/**/*.cs,src/UI/DnDQueryPrompter/**/*.cs,src/Components/DndUi/UserInteractionHandlers/**/*.cs"
---

# User Interaction & Query Prompting Conventions

Two systems bridge business logic with Blazor UI without coupling them: **User Interactions** (push-based, for commands) and **Query Prompters** (pull-based, for queries via dialogs). The MAUI host wires these together in `MainLayout`.

## 1. User Interactions (`src/Business/DnDUserInterations/`)

Business-layer contracts for requesting information from the user during command execution.

### Contracts

- `IUserInteraction` / `IUserInteraction<TAnswer>` — marker interfaces. Every interaction has a `Guid InteractionId`.
- `UserInteractionBase<TAnswer>` — base **record** (not class) with auto-generated `InteractionId`.
- Concrete interactions (e.g., `MartialAttackRollResultRequestInteraction`) are records inheriting `UserInteractionBase<TAnswer>`.

### Service

- `IUserInteractionService` — singleton. Exposes:
  - `event Func<IUserInteraction, Task>? InteractionRaised` — UI subscribes to this.
  - `Task<IQueryResponse<TResponse>> RequestAsync<TResponse>(IUserInteraction<TResponse> request)` — called by command handlers, blocks until UI responds.
  - `void CompleteSuccess<TResponse>(Guid id, TResponse response)` — UI calls this on confirmation.
  - `void CompleteCanceled(Guid id)` — UI calls this on cancel/close.
- Implementation (`UserInteractionService`) uses `ConcurrentDictionary<Guid, TaskCompletionSource<object>>` for pending interactions.
- Cancellation is modeled as `OperationCanceledException` set on the `TaskCompletionSource`.
- Responses are wrapped in `QueryResponse<T>.Success(...)` / `.Canceled(...)`.

## 2. Query Prompters (`src/UI/DnDQueryPrompter/`)

UI-layer project that implements query handlers and dialog modals for interactive queries.

### Dialog Service

- `IDialogServiceProvider` — interface (defined in `DnDQueries` project). Provides `SetDialogService(IDialogService)` / `GetDialogService()` for MudBlazor dialog access.
- `DialogServiceProvider` — implementation in DnDQueryPrompter. Registered as singleton.

### Query Handlers

- Query handlers that need UI dialogs live here (e.g., `SaveRollResultQueryHandler`).
- They inherit `QueryHandlerBase<TQuery, TResponse>` and inject `IDialogServiceProvider` to show MudBlazor dialogs.
- They use `_dialogServiceProvider.GetDialogService().ShowAsync<TDialog>(...)` to display a dialog, then await `dialog.Result`.

### Interaction Modals

- Modal components (e.g., `MartialAttackRollResultRequestInteractionModal`) are Razor components with code-behind.
- They receive domain data via `[Parameter]` and render using MudBlazor + shared components.

## 3. MAUI Host Handlers (`src/Components/DndUi/UserInteractionHandlers/`)

The MAUI host wires user interactions to modals via **partial class extensions of `MainLayout`**.

### Pattern

- Each handler is a separate file that extends `public partial class MainLayout` (namespace `DndUi.Components.Layout`).
- The MainLayout subscribes to `IUserInteractionService.InteractionRaised` and dispatches to typed handler methods.
- Handler methods:
  1. Resolve the required entities from `IFightContext`.
  2. Build `DialogParameters<TModal>` with the relevant data.
  3. Call `Dialogs.ShowAsync<TModal>(title, parameters, options)`.
  4. Await `dialog.Result`.
  5. Call `UserInteractionService.CompleteSuccess(...)` or `UserInteractionService.CompleteCanceled(...)`.

### Example

```csharp
private async Task HandleInteractionAsync(MartialAttackRollResultRequestInteraction interaction)
{
    var caster = _fightContext[interaction.CasterId] ?? throw new NullReferenceException(...);
    var attackTemplate = caster.MartialAttacks.GetTemplateByIdOrDefault(interaction.AttackId) ?? throw ...;
    var rollableResult = attackTemplate.GetRollableResult();

    var options = new DialogOptions { BackdropClick = false, CloseButton = true };
    var parameters = new DialogParameters<MartialAttackRollResultRequestInteractionModal>
    {
        { x => x.MartialAttackRollResult, rollableResult }
    };

    var dialog = await Dialogs.ShowAsync<MartialAttackRollResultRequestInteractionModal>(..., parameters, options);
    var result = await dialog.Result;

    if (result?.Canceled ?? true)
    {
        UserInteractionService.CompleteCanceled(interaction.InteractionId);
        return;
    }
    UserInteractionService.CompleteSuccess(interaction.InteractionId, rollableResult);
}
```
