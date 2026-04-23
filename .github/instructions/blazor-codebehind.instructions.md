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
