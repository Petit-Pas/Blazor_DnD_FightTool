---
name: undoable-mediator
description: Use the UndoableMediator library for mediator pattern with undo/redo and sub-command trees in .NET applications. Use when creating commands, queries, handlers, or working with undo/redo history.
---

# UndoableMediator

.NET mediator with built-in undo/redo and sub-command trees.
Singleton, not thread-safe — designed for desktop / single-user apps.

## Rules

- Commands mutate state and are always added to undo history on `RequestStatus.Success`.
- Queries are read-only — no undo.
- Sub-commands form a tree: undoing a parent cascades to children automatically.
- Handlers must call `base.UndoAsync` / `base.RedoAsync` to propagate to sub-commands, or handle them manually.
- Store old state in the command during `ExecuteAsync` so `UndoAsync` can restore it.

## Base Classes

| To create | Inherit from | Handler base |
|-----------|-------------|--------------|
| Command (no return) | `CommandBase` | `CommandHandlerBase<TCommand>` |
| Command (returns T) | `CommandBase<T>` | `CommandHandlerBase<TCommand, T>` |
| Query (returns T) | `QueryBase<T>` | `QueryHandlerBase<TQuery, T>` |

Handler constructors take `IUndoableMediator mediator` → `base(mediator)`.
The mediator is available as `protected readonly IUndoableMediator _mediator`.

## IUndoableMediator API

```csharp
Task<ICommandResponse<T>> SendAsync<T>(ICommand<T> command);
Task<ICommandResponse<T>> SendAsSubCommandAsync<T>(ICommand<T> subCommand, ICommand parentCommand);
Task<IQueryResponse<T>> QueryAsync<T>(IQuery<T> query);
Task<bool> UndoLastCommandAsync();
Task<bool> RedoLastUndoneCommandAsync();
int HistoryLength { get; }
int RedoHistoryLength { get; }
```

## Patterns

### Simple command

```csharp
public class ChangeAgeCommand : CommandBase
{
    public int NewAge { get; }
    public int OldAge { get; set; }
    public ChangeAgeCommand(int newAge) => NewAge = newAge;
}

public class ChangeAgeCommandHandler : CommandHandlerBase<ChangeAgeCommand>
{
    public ChangeAgeCommandHandler(IUndoableMediator mediator) : base(mediator) { }

    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(ChangeAgeCommand command)
    {
        command.OldAge = Model.Age;
        Model.Age = command.NewAge;
        return Task.FromResult(CommandResponse.Success());
    }

    public override async Task UndoAsync(ChangeAgeCommand command)
    {
        await base.UndoAsync(command);
        Model.Age = command.OldAge;
    }

    public override async Task RedoAsync(ChangeAgeCommand command)
    {
        Model.Age = command.NewAge;
        await base.RedoAsync(command);
    }
}
```

### Sub-commands

```csharp
public override async Task<ICommandResponse<NoResponse>> ExecuteAsync(ParentCommand command)
{
    await _mediator.SendAsSubCommandAsync(new ChildA(...), parentCommand: command);
    await _mediator.SendAsSubCommandAsync(new ChildB(...), parentCommand: command);
    return CommandResponse.Success();
}
// Undo/Redo propagation is automatic — no override needed.
```

### Re-execute redo strategy
Mostly usable when a model on which the commant relied has changed state, potentially invalidating the child commands.
```csharp
public override async Task RedoAsync(MyCommand command)
{
    ClearSubCommands(command);
    await ExecuteAsync(command);
}
```

## Response factories
Best used with a static using for `CommandResponse` and `QueryResponse` to avoid verbosity in handlers.
```csharp
CommandResponse.Success() / .Success<T>(value) / .Failed() / .Canceled()
QueryResponse<T>.Success(value) / .Failed(value) / .Canceled(value)
```

## DI registration

```csharp
builder.Services.ConfigureMediator(options =>
{
    options.AssembliesToScan = new[] { typeof(MyCommand).Assembly };
    options.CommandHistoryMaxSize = 64;      // default
    options.RedoHistoryMaxSize = 32;         // default
    options.ShouldScanAutomatically = false; // default
});
```

## Common mistakes

| Mistake | Symptom |
|---------|-----|
| Not calling `base.UndoAsync` | Sub-commands are not undone |
| Using `SendAsync` instead of `SendAsSubCommandAsync` for child operations. Should almost always be the case inside of the handler of another command | Sub-commands won't cascade undo/redo |
| Not saving old state in the command during `ExecuteAsync` | Cannot restore in `UndoAsync` |
| Returning `Failed` / `Canceled` when state was already mutated | Command won't be added to history — undo impossible |
