---
applyTo: "**/*.razor.cs"
---

# Blazor Code-Behind Conventions (.razor.cs)

- **Partial classes**: All code-behind files declare `public partial class {ComponentName}` — the class name matches the `.razor` file name exactly.
- **Namespace**: Matches the folder structure relative to the project's root namespace.
- **Parameters**: Use `[Parameter]` attribute. Declare with `public` access and `{ get; set; }`.
  - `EventCallback` / `EventCallback<T>` for event parameters.
  - Default values on parameters where appropriate (e.g., `Variant.Filled`, `Color.Default`).
- **Base class**: If the component inherits `StylableComponentBase`, the code-behind class has `Class` and `Style` parameters inherited — do not redeclare them.
- **Component lifecycle**: Override `OnParametersSet()`, `OnInitializedAsync()`, etc. as needed. Call `base.OnParametersSet()`.
- **Internal properties**: Use `internal` or `protected` for non-parameter properties that are referenced in markup (e.g., `internal virtual string Icon`).
- **Presets / derived components**: Concrete button types (e.g., `AddButton`, `DeleteButton`) inherit from a base component class and set properties in the constructor — no `.razor` file needed for these.
- **Validation**: Expose a `public bool Validate()` method when the component needs external validation triggering.
- **No business logic**: Code-behind files should only contain UI logic (parameter handling, validation display, state management). Business logic goes through the mediator.

## IDisposable & Event Cleanup

- Components that subscribe to events (domain events, `IDiceRollNotifier.StateChanged`, `IFightContext.OnFighterUpdated`, etc.) must implement `IDisposable` and unsubscribe in `Dispose()`.
- Declare `IDisposable` in the code-behind partial class.

```csharp
public partial class MyComponent : IDisposable
{
    protected override void OnInitialized()
    {
        _fightContext.OnFighterUpdated += OnFighterUpdated;
    }

    public void Dispose()
    {
        _fightContext.OnFighterUpdated -= OnFighterUpdated;
    }
}
```

## IDiceRollNotifier / IDiceRollSubscriber Pattern

Used to orchestrate multi-dice roll dialogs. A parent dialog provides an `IDiceRollNotifier` as a cascading value; child `DiceRollResultInputComponent` instances implement `IDiceRollSubscriber` and register themselves.

- **Parent dialog**: Holds a `DiceRollNotifier` instance (concrete type in `FightBlazorComponents`). Passes it down as `[CascadingParameter]` typed as `IDiceRollNotifier`. Listens to `IDiceRollNotifier.StateChanged` to call `StateHasChanged()`. Disposes by unsubscribing.
- **Child components** (`DiceRollResultInputComponent`): Call `notifier.Subscribe(this)` in `OnInitialized` and `notifier.Unsubscribe(this)` in `Dispose()`. The parent's Roll button is enabled only when `IDiceRollNotifier.CanRoll == true` (i.e., at least one subscriber has not yet rolled).

## GlobalEditContext

`IGlobalEditContext` (implemented by `GlobalEditContext` in `CharacterSheetBlazorComponents`, registered as `Scoped`) holds the currently-edited entity across navigation steps. Inject via `[Inject]`; do not pass entity state through route parameters or long parameter chains across multiple pages.

## IMapper in Components

When a component needs an in-memory snapshot for undo (e.g., editing a character), inject `IMapper` via `[Inject]` and call:
- `_mapper.Copy<T>(entity)` — exact copy with same Ids, used as the undo baseline.
- `_mapper.Clone<T>(entity)` — new Ids, used when duplicating an entity.
