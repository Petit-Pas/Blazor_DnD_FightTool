---
applyTo: "**/*.razor.css"
---

# Blazor CSS Isolation Conventions

- **Scoped styles**: Each `.razor.css` file is automatically scoped to its paired `.razor` component.
- **MudBlazor overrides**: Use `::deep` combinator to target MudBlazor internal elements (e.g., `::deep .mud-chip`, `::deep .mud-badge-wrapper > .mud-badge`).
- **Class naming**: Use descriptive, component-scoped class names (e.g., `ability-score-chip`, `ability-score-badge`).
- **Sizing**: Use fixed units (`px`, `em`) for component-specific sizing. Use CSS transforms for positioning adjustments.
- **Keep minimal**: Only include styles specific to this component. Shared/global styles go in `wwwroot/css/app.css`.
