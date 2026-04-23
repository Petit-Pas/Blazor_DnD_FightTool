---
applyTo: "src/Business/DnDActions/**/*.cs"
---

# Command & Command Handler Conventions (UndoableMediator)

This project uses the **UndoableMediator** library (v2.0.0-alpha1). Refer to the `undoable-mediator` skill for full API reference.

## Commands

- **Location**: Each command+handler pair lives in its own folder: `{Category}Actions/{ActionName}/`.
- **Base classes**: Commands inherit from one of:
  - `CommandBase` — bare command (from UndoableMediator)
  - `TargetCommandBase` — has a `Guid TargetId` (character receiving the effect)
  - `CasterCommandBase` — has a `Guid CasterId` (character performing the action)
  - `CasterTargetCommandBase` — has both `CasterId` and `TargetId`
- **Immutable inputs**: Command properties set via constructor are get-only (`{ get; }`). Mutable state for undo (e.g., `CorrectedAmount`) uses `{ get; set; }`.
- **Naming**: `{Action}Command` and `{Action}CommandHandler`.

## Handlers

- **Base class**: `CommandHandlerBase<TCommand>` from UndoableMediator.
- **Constructor**: Always takes `IUndoableMediator mediator` + any required services. Calls `base(mediator)`. The mediator is available as `protected readonly IUndoableMediator _mediator`.
- **Methods** (UndoableMediator v2.0.0-alpha1 API):
  - `ExecuteAsync(TCommand command)` → returns `Task<ICommandResponse<NoResponse>>`
  - `UndoAsync(TCommand command)` → returns `Task` (call `base.UndoAsync(command)` to propagate to sub-commands)
  - `RedoAsync(TCommand command)` → returns `Task` (call `base.RedoAsync(command)` to propagate to sub-commands)
- **Sub-commands**: Use `await _mediator.SendAsSubCommandAsync(childCommand, parentCommand: command)`. This replaces the old `command.AddToSubCommands` + `_mediator.Execute` pattern.
- **Clearing sub-commands**: Use the handler method `ClearSubCommands(command)` (not `command.SubCommands.Clear()`).
- **Redo strategy**: When the model may have changed between execute and redo, clear sub-commands and re-execute:
  ```csharp
  public override async Task RedoAsync(MyCommand command)
  {
      ClearSubCommands(command);
      await ExecuteAsync(command);
  }
  ```
- **Return**: Use `CommandResponse.Success()` for successful execution. Use `new CommandResponse(RequestStatus)` for non-success statuses.
- **Errors**: Throw exceptions (not `CommandResponse.Failed()`) when a required entity is missing from `IFightContext`.
- **Fighter state notification**: When a handler mutates fighter state in-place (e.g., HP changes), call `_fightContext.NotifyFighterUpdated(command.TargetId)` in both `ExecuteAsync` and `UndoAsync` so that UI components re-render.
