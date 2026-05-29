---
applyTo: "src/Business/DnDActions/**/*.cs"
---

# Command & Command Handler Conventions (UndoableMediator)

This project uses the **UndoableMediator** library (v2.0.0-alpha1). Refer to the `undoable-mediator` skill for full API reference.

## Command Classification: Orchestrator vs Atomic

Every command handler MUST be either an **orchestrator** or an **atomic** operation — never both.

### Atomic handlers
- Perform a **single, direct state mutation** (e.g., mutate HP, remove from a collection, update a field).
- Have **no sub-commands**.
- Implement `UndoAsync` by directly reversing their own mutation.
- Do **NOT** call `base.UndoAsync` / `base.RedoAsync` — return `Task.CompletedTask` directly.
- May inject domain services (`IFightContext`, etc.) and call their mutating methods.
- Example: `RemoveFromFightAtomicCommandHandler` — removes from context in Execute, restores from stash in Undo.

### Orchestrator handlers
- Coordinate multiple sub-commands in a meaningful sequence.
- Have **no direct state mutations** — all mutations happen inside sub-commands.
- `UndoAsync` has NO custom logic: it ONLY calls `base.UndoAsync(command)` to cascade undo to sub-commands.
- `RedoAsync` always uses: `ClearSubCommands(command); await ExecuteAsync(command);`
- May inject services **only to read or validate** (e.g., null-check, current-turn lookup) — never to mutate.
- May return early on validation failure before dispatching any sub-commands.
- Example: `RemoveFromFightCommandHandler` — validates existence, routes conditionally, then delegates to `RemoveFromFightAtomicCommand`, `SetCurrentFighterCommand`, `WriteLogCommand`.

### Atomic naming and co-location

When a domain action has both an **orchestrator** and an **atomic** variant, the atomic class is named with an `Atomic` suffix: `XyzAtomicCommand` / `XyzAtomicCommandHandler`. Both the orchestrator pair and the atomic pair **live in the same folder**:

```
LooseHp/
  LooseHpCommand.cs               ← orchestrator command
  LooseHpCommandHandler.cs        ← orchestrator handler (dispatches atomic + log)
  LooseHpAtomicCommand.cs         ← atomic command
  LooseHpAtomicCommandHandler.cs  ← atomic handler (pure mutation, no log)
```

The orchestrator is the **public API** consumers should call. The atomic is an **implementation detail** — only dispatched as a sub-command by its own orchestrator.

### Sub-command naming convention
Sub-commands that are **only ever dispatched as children** of an orchestrator (never called top-level) should reflect their atomic role explicitly in the name via the `Atomic` suffix.

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
- **Query calls**: Use `await _mediator.QueryAsync(query)` and return the `IQueryResponse<T>` directly from private helper methods — do **not** translate the response to `T?` or throw inside the helper. Check `.Status` and read `.Response` at the call site.
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
- **Leaf vs orchestrator**: Orchestrators that only delegate to sub-commands (e.g., `TakeDamageCommandHandler`) do **not** need their own log entries — the sub-commands handle it. Leaf orchestrators (e.g., `LooseHpCommandHandler`) emit a single log entry after their atomic sub-command completes, using the `CorrectedAmount` from the returned atomic command instance.
