# Research: Undo/Redo Buttons

## R-001: UndoableMediator Event API (prerequisite)

**Decision**: The UndoableMediator library (currently `2.0.0-alpha2`) must be updated to expose three `Action` events on `IUndoableMediator`:
- `event Action? OnCommandExecuted`
- `event Action? OnCommandUndone`
- `event Action? OnCommandRedone`

These events fire after the respective operation completes successfully. The Blazor component subscribes to these events to trigger `StateHasChanged`.

**Rationale**: Polling `HistoryLength` / `RedoHistoryLength` is not viable in Blazor — there is no render loop. Event-driven state updates are the standard Blazor pattern (same as `ICombatTurnService.OnChanged`).

**Alternatives considered**:
- Wrapper service around `IUndoableMediator` that raises events — rejected because it adds an unnecessary layer and forces all callers to use the wrapper instead of the real mediator.
- `INotifyPropertyChanged` on the mediator — rejected as overkill for three discrete state changes.

**Resolution**: Publish a new UndoableMediator pre-release (`2.0.0-alpha3` or similar) with the three events before starting implementation. The feature is blocked until this is done.

---

## R-002: MudBlazor Icon Button for Undo/Redo

**Decision**: Use `MudIconButton` with `Icons.Material.Filled.Undo` and `Icons.Material.Filled.Redo` from MudBlazor v8.x. Disable via the `Disabled` parameter.

**Rationale**: MudBlazor already includes Material Design icons. The standard undo/redo icons (curved arrows) are `Undo` and `Redo` in the Material Filled set. Using `MudIconButton` provides built-in disabled styling (greyed out).

**Alternatives considered**:
- Custom SVG icons — rejected; Material icons match the spec description (circular curved arrows) and are already available.
- `MudButton` with text — rejected; the spec requires icon-only buttons.

---

## R-003: Placement within CombatStatusComponent

**Decision**: Add the undo/redo buttons to the existing `CombatStatusComponent` as a row of two `MudIconButton`s positioned at the top-right of the panel. This avoids creating a separate component.

**Rationale**: The spec states "located in the component that displays rounds and turns (bottom right of the fight page)" and "statically at the top-right corner of the panel." The combat status panel already has this layout identity. Adding buttons there keeps the feature localized.

**Alternatives considered**:
- Separate `UndoRedoButtonsComponent` embedded inside `CombatStatusComponent` — viable but premature abstraction for two buttons. If complexity grows later, it can be extracted.
- Placing buttons in the page layout outside the panel — rejected; spec explicitly says within the panel.

---

## R-004: Component Subscription Lifecycle

**Decision**: Subscribe to the three mediator events in `OnInitialized()` and unsubscribe in `Dispose()`. On event fire, call `InvokeAsync(StateHasChanged)` (same pattern as the existing `_combatTurnService.OnChanged` subscription).

**Rationale**: Events may fire from non-UI threads (mediator dispatch chain). `InvokeAsync` marshals to the Blazor synchronization context. The component already implements `IDisposable` — we just add three more unsubscriptions.

**Alternatives considered**:
- Using `IObservable<T>` / Rx — rejected; adds dependency and complexity for three simple events.

---

## R-005: No New Project Required

**Decision**: All changes fit within existing projects. No new `.csproj` needed.

**Rationale**: The UI change is in `FightBlazorComponents/CombatStatus/`. The mediator library update is external (NuGet package). No new domain types, no new command handlers.

**Alternatives considered**: N/A — the scope is minimal.
