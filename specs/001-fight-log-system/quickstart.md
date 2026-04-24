# Quickstart: DnD Log System

**Feature**: 001-fight-log-system

## How to use the log system in a command handler

### 1. Inject `IDnDLogService`

```csharp
public class MyCommandHandler : CommandHandlerBase<MyCommand>
{
    private readonly IDnDLogService _logService;

    public MyCommandHandler(IUndoableMediator mediator, IDnDLogService logService) : base(mediator)
    {
        _logService = logService;
    }
}
```

### 2. Open a block, write entries, close the block

```csharp
public override async Task<ICommandResponse<NoResponse>> ExecuteAsync(MyCommand command)
{
    _logService.OpenBlock("Martial Attack");

    // Write a log entry as a sub-command (so undo hides it automatically)
    await _mediator.SendAsSubCommandAsync(
        new WriteLogCommand("PlayerA attacks Goblin"),
        parentCommand: command);

    _logService.OpenScope(); // indent subsequent entries

    await _mediator.SendAsSubCommandAsync(
        new WriteLogCommand("[hover:d20 roll: 15 + 3 modifier][b]18[/b][/hover] to hit"),
        parentCommand: command);

    await _mediator.SendAsSubCommandAsync(
        new WriteLogCommand("[c:fire][b]6[/b] fire damage[/c]"),
        parentCommand: command);

    _logService.CloseScope();
    _logService.CloseBlock();

    return CommandResponse.Success();
}
```

### 3. Formatting reference

| Tag | Example | Renders as |
|-----|---------|------------|
| `[b]...[/b]` | `[b]18[/b]` | **18** |
| `[c:token]...[/c]` | `[c:fire]6 fire damage[/c]` | <span style="color:fire">6 fire damage</span> |
| `[hover:text]...[/hover]` | `[hover:d20=15+3]18[/hover]` | 18 (tooltip: "d20=15+3") |

Tags nest: `[c:fire][b]10[/b] fire damage[/c]` → bold 10 + normal "fire damage", all in fire color.

### 4. Semantic color tokens

Damage types: `bludgeoning`, `piercing`, `slashing`, `acid`, `cold`, `fire`, `force`, `lightning`, `necrotic`, `poison`, `psychic`, `radiant`, `thunder`

Physical variants (same color as base): `bludgeoning-silver`, `piercing-silver`, `slashing-silver`, `bludgeoning-magic`, `piercing-magic`, `slashing-magic`

Special: `heal`

Only tokens defined in the `LogColorToken` enum are accepted. If a token is not in the enum, the entire `[c:...]...[/c]` tag is rendered as literal text (useful for spotting typos during development).

### 5. Undo/redo

No special handling needed. Because `WriteLogCommand` is dispatched as a sub-command, undoing the parent command automatically hides all its log entries. Redoing restores them.

### 6. UI subscription (for Blazor components)

```csharp
[Inject] private IDnDLogService LogService { get; set; } = null!;

protected override void OnInitialized()
{
    LogService.OnChanged += HandleLogChanged;
}

private void HandleLogChanged() => InvokeAsync(StateHasChanged);

public void Dispose()
{
    LogService.OnChanged -= HandleLogChanged;
}
```
