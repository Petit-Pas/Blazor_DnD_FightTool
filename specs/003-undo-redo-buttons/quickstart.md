# Quickstart: Undo/Redo Buttons

## Prerequisites

1. **UndoableMediator library updated** to a version exposing `OnCommandExecuted`, `OnCommandUndone`, and `OnCommandRedone` events on `IUndoableMediator`. Update the NuGet reference in all `.csproj` files that reference it.

## Build & Run

```powershell
# Build solution
dotnet build DnDFightTool.slnx

# Run web host (for quick iteration)
dotnet run --project src/Components/DndUi.Web/DndUi.Web.csproj

# Run all tests
dotnet test DnDFightTool.slnx
```

## Implementation Steps (high-level)

### Step 1: Update UndoableMediator NuGet Package

Update `PackageReference` in these projects from `2.0.0-alpha2` to the new version:
- `src/Components/DndUi/DndUi.csproj`
- `src/Components/DndUi.Web/DndUi.Web.csproj`
- `src/Business/DnDActions/DnDActions.csproj`
- `src/Business/DnDQueries/DnDQueries.csproj`
- `src/Infrastructure/Extensions/Extensions.csproj`

### Step 2: Modify CombatStatusComponent.razor

Add two `MudIconButton` elements in a row at the top-right of the panel:
- Undo: `Icons.Material.Filled.Undo`, disabled when `CanUndo` is false
- Redo: `Icons.Material.Filled.Redo`, disabled when `CanRedo` is false

### Step 3: Modify CombatStatusComponent.razor.cs

- Add `CanUndo` / `CanRedo` computed properties
- Add click handlers: `OnUndoClick()` → `_mediator.UndoLastCommandAsync()`; `OnRedoClick()` → `_mediator.RedoLastUndoneCommandAsync()`
- Subscribe to the three mediator events in `OnInitialized()`
- Unsubscribe in `Dispose()`
- Event handler: `InvokeAsync(StateHasChanged)`

### Step 4: Modify CombatStatusComponent.razor.css

Add styling for the button row (top-right positioning within the panel).

### Step 5: Add Tests (optional — bUnit)

Test that:
- Undo button calls `UndoLastCommandAsync` on click
- Redo button calls `RedoLastUndoneCommandAsync` on click
- Buttons are disabled when `HistoryLength` / `RedoHistoryLength` is 0
- Buttons become enabled after events fire

## Files Changed

| File | Change |
|------|--------|
| `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor` | Add undo/redo icon buttons |
| `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.cs` | Add event subscriptions, handlers, properties |
| `src/UI/FightBlazorComponents/CombatStatus/CombatStatusComponent.razor.css` | Add button row layout |
| `src/Components/DndUi/DndUi.csproj` | Update UndoableMediator version |
| `src/Components/DndUi.Web/DndUi.Web.csproj` | Update UndoableMediator version |
| `src/Business/DnDActions/DnDActions.csproj` | Update UndoableMediator version |
| `src/Business/DnDQueries/DnDQueries.csproj` | Update UndoableMediator version |
| `src/Infrastructure/Extensions/Extensions.csproj` | Update UndoableMediator version |

## Verification

1. Build passes: `dotnet build DnDFightTool.slnx`
2. Existing tests pass: `dotnet test DnDFightTool.slnx`
3. Manual verification: open fight page → both buttons greyed out → perform action → undo button active → click undo → redo button active → click redo → state restored
