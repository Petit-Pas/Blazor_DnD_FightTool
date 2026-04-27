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

## Logging

Handlers **MUST** produce log entries for user-visible actions via write-log sub-commands. Refer to the `dnd-logging` skill for the full API reference, formatting tags, and color tokens.

- **Dependency**: Do **not** inject `IDnDLogService` in command handlers. Block/scope management is done via `OpenBlockCommand`, `CloseBlockCommand`, `OpenScopeCommand`, `CloseScopeCommand` sub-commands. Log entries are added via `WriteLogCommand` sub-commands. All logging goes through `_mediator`.
- **Log entries**: Send a `WriteLogCommand` as a sub-command.
- **Private methods**: Every `_mediator.SendAsSubCommandAsync` call for a log command (`WriteLogCommand`, `OpenBlockCommand`, `CloseBlockCommand`, `OpenScopeCommand`, `CloseScopeCommand`) MUST be extracted into a dedicated private `async Task` method. Name it after the action it performs: `LogHpLoss`, `OpenTurnLog`, `CloseTurnLog`, `LogRoundHeader`, `OpenAttackLog`, etc. `ExecuteAsync` calls those methods — it never dispatches log sub-commands inline.
  ```csharp
  // ✅ correct
  private async Task LogHpLoss(string fighterName, LooseHpCommand command)
  {
      await _mediator.SendAsSubCommandAsync(
          new WriteLogCommand($"[b]{fighterName}[/b] loses [b]{command.CorrectedAmount}[/b] HPs"),
          parentCommand: command);
  }
  ```
- **Undo/redo**: `WriteLogCommandHandler` hides entries on undo and shows them on redo automatically. No additional handler logic needed.
- **Blocks**: Top-level orchestrating handlers (e.g., `ExecuteMartialAttackCommandHandler`) open/close blocks via `OpenBlockCommand` / `CloseBlockCommand` sub-commands dispatched through `_mediator`. Do **not** inject `IDnDLogService` for this purpose.
- **Scopes**: Indent nested detail entries via `OpenScopeCommand` / `CloseScopeCommand` sub-commands dispatched through `_mediator`.
- **Formatting tags**: `[b]...[/b]` (bold), `[c:token]...[/c]` (color), `[hover:tooltip]...[/hover]` (tooltip). See the `dnd-logging` skill for the full token list.
- **Leaf vs orchestrator**: Leaf handlers (e.g., `LooseHpCommandHandler`) emit a single log entry. Orchestrators (e.g., `TakeDamageCommandHandler`) that only delegate to sub-commands do NOT need their own log entries — the sub-commands handle it.
