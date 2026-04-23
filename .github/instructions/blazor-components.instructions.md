---
applyTo: "**/*.razor"
---

# Blazor Component Conventions (.razor)

- **UI framework**: MudBlazor (v8.x). Use `MudBlazor` components (`MudButton`, `MudTextField`, `MudChip`, `MudIcon`, `MudBadge`, etc.) — not raw HTML controls.
- **Base class**: Components that accept `Class` and `Style` parameters inherit from `StylableComponentBase` via `@inherits StylableComponentBase`.
- **Code-behind**: Use partial class code-behind files (`.razor.cs`) for all logic. Keep `.razor` files markup-only or with minimal inline expressions.
- **Imports**: Common `@using` directives go in `_Imports.razor` per project. Component-specific usings go at the top of the `.razor` file.
- **Generics**: Use `@typeparam T where T : ...` for generic components.
- **Conditional rendering**: Use `@if` blocks for conditional markup. Avoid ternary in complex attribute expressions.
- **Event binding**: Use `OnClick=OnClick` (no `@` prefix for component parameters), `ValueChanged="@(async (string value) => { ... })"` for inline async handlers.
- **CSS classes**: Use MudBlazor utility classes and custom CSS classes. Combine with string concatenation, not interpolation where possible.
- **Extension methods**: Use existing extensions like `"css-class".When(condition)` from `Extensions` project for conditional CSS classes.
