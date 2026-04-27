# Data Model: Undo/Redo Buttons

## Overview

This feature introduces **no new domain entities, commands, or queries**. It adds UI-only state (button enabled/disabled) derived from existing `IUndoableMediator` properties. The only external change is a library update.

---

## External Dependency Change

### IUndoableMediator (library update)

The `IUndoableMediator` interface in the UndoableMediator NuGet package gains three events:

```csharp
public interface IUndoableMediator
{
    // Existing members (unchanged):
    Task<ICommandResponse<T>> SendAsync<T>(ICommand<T> command);
    Task<ICommandResponse<T>> SendAsSubCommandAsync<T>(ICommand<T> subCommand, ICommand parentCommand);
    Task<IQueryResponse<T>> QueryAsync<T>(IQuery<T> query);
    Task<bool> UndoLastCommandAsync();
    Task<bool> RedoLastUndoneCommandAsync();
    int HistoryLength { get; }
    int RedoHistoryLength { get; }

    // NEW — added by library update:
    event Action? OnCommandExecuted;
    event Action? OnCommandUndone;
    event Action? OnCommandRedone;
}
```

**Version**: Update from `2.0.0-alpha2` → `2.0.0-alpha3` (or whichever version ships these events).

---

## UI State (derived, not persisted)

### CombatStatusComponent — Undo/Redo Button State

| Property | Type | Source | Description |
|----------|------|--------|-------------|
| `CanUndo` | `bool` | `_mediator.HistoryLength > 0` | Enables/disables the undo button |
| `CanRedo` | `bool` | `_mediator.RedoHistoryLength > 0` | Enables/disables the redo button |

These are computed properties recalculated on each render, triggered by event subscriptions.

---

## State Transitions

```
[Any Command Executed] → OnCommandExecuted fires
    → CanUndo = true (HistoryLength increased)
    → CanRedo = false (RedoHistory cleared by mediator)

[Undo Clicked] → UndoLastCommandAsync() → OnCommandUndone fires
    → CanUndo = (HistoryLength > 0)
    → CanRedo = true (RedoHistoryLength increased)

[Redo Clicked] → RedoLastUndoneCommandAsync() → OnCommandRedone fires
    → CanUndo = true (HistoryLength increased)
    → CanRedo = (RedoHistoryLength > 0)
```

---

## Relationships

```
CombatStatusComponent
    ├── subscribes to → IUndoableMediator.OnCommandExecuted
    ├── subscribes to → IUndoableMediator.OnCommandUndone
    ├── subscribes to → IUndoableMediator.OnCommandRedone
    ├── reads → IUndoableMediator.HistoryLength
    ├── reads → IUndoableMediator.RedoHistoryLength
    ├── calls → IUndoableMediator.UndoLastCommandAsync()
    └── calls → IUndoableMediator.RedoLastUndoneCommandAsync()
```

---

## Validation Rules

- None — no user input is captured. Buttons are either enabled or disabled based on history state.

---

## No New Files in Domain or Business Layers

This feature is purely a UI enhancement. The undo/redo infrastructure already exists in the mediator library.
