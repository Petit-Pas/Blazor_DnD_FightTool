---
name: dnd-logging
description: Use the DnD log system for structured game-event logging. Use when creating or updating command handlers that produce log entries, working with log blocks/scopes, BBCode formatting tags, or semantic color tokens.
---

# DnD Fight Log System

Structured game-event logging for DnD fight actions. Handlers emit log entries as sub-commands; the UI renders them with rich formatting, grouping, and undo/redo support.

## Architecture

```
Domain (Logs)          → IDnDLogService, LogBlock, LogEntry, LogColorToken
Business (DnDActions)  → WriteLogCommand / WriteLogCommandHandler
UI (FightBlazorComponents) → DnDLogComponent, LogBlockComponent, LogEntryComponent, LogTokenParser
```

- `IDnDLogService` is registered as a **singleton**.
- Log entries are created via `WriteLogCommand` sent as **sub-commands** — undo cascades automatically.

## IDnDLogService API

```csharp
IReadOnlyList<LogBlock> Blocks { get; }
event Action? OnChanged;

void OpenBlock(string name);    // Start a named group
void CloseBlock();              // End the current group
void OpenScope();               // Increment indent level
void CloseScope();              // Decrement indent level
Guid AddEntry(string content);  // Create a new LogEntry in the current block; returns the entry GUID
void Hide(Guid entryId);       // Mark entry as hidden (undo)
void Show(Guid entryId);       // Mark entry as visible (redo)
bool IsHidden(Guid entryId);   // Check visibility
void Clear();                   // Remove all blocks, entries, and hidden state
```

## WriteLogCommand

```csharp
// Command
public class WriteLogCommand : CommandBase
{
    public string Content { get; }
    public Guid? LogEntryId { get; set; }  // Set by handler on execute
}

// Usage in a handler's ExecuteAsync:
await _mediator.SendAsSubCommandAsync(
    new WriteLogCommand("log content here"),
    parentCommand: command);
```

The handler automatically:
- **Execute**: calls `AddEntry(content)`, stores the entry ID
- **Undo**: calls `Hide(entryId)`
- **Redo**: calls `Show(entryId)`

## Formatting Tags (BBCode-like)

| Tag | Renders as | Example |
|-----|-----------|---------|
| `[b]...[/b]` | `<strong>` | `[b]Goblin[/b]` |
| `[c:token]...[/c]` | `<span>` with CSS color var | `[c:fire]8 Fire[/c]` |
| `[hover:tooltip]...[/hover]` | `<span title="tooltip">` | `[hover:d20 = 14, mods = STR+2]16[/hover]` |

Tags can be nested: `[c:fire][b]8[/b] Fire[/c]`

Unknown `[c:xxx]` tokens where `xxx` doesn't match a `LogColorToken` value are rendered as literal text.

## Color Tokens

All 20 values of the `LogColorToken` enum, each mapping to CSS variable `--dnd-color-{kebab-case}`:

| Token | CSS Variable | Usage |
|-------|-------------|-------|
| `bludgeoning` | `--dnd-color-bludgeoning` | Physical damage |
| `piercing` | `--dnd-color-piercing` | Physical damage |
| `slashing` | `--dnd-color-slashing` | Physical damage |
| `bludgeoning-silver` | `--dnd-color-bludgeoning-silver` | Silvered weapon |
| `piercing-silver` | `--dnd-color-piercing-silver` | Silvered weapon |
| `slashing-silver` | `--dnd-color-slashing-silver` | Silvered weapon |
| `bludgeoning-magic` | `--dnd-color-bludgeoning-magic` | Magical weapon |
| `piercing-magic` | `--dnd-color-piercing-magic` | Magical weapon |
| `slashing-magic` | `--dnd-color-slashing-magic` | Magical weapon |
| `acid` | `--dnd-color-acid` | Elemental |
| `cold` | `--dnd-color-cold` | Elemental |
| `fire` | `--dnd-color-fire` | Elemental |
| `force` | `--dnd-color-force` | Elemental |
| `lightning` | `--dnd-color-lightning` | Elemental |
| `necrotic` | `--dnd-color-necrotic` | Damage type |
| `poison` | `--dnd-color-poison` | Damage type |
| `psychic` | `--dnd-color-psychic` | Damage type |
| `radiant` | `--dnd-color-radiant` | Damage type |
| `thunder` | `--dnd-color-thunder` | Damage type |
| `heal` | `--dnd-color-heal` | Healing |

Light/dark theme variants are defined in `src/Components/DndUi.Shared/wwwroot/css/log-colors.css` using `:root` (light) and `.dnd-dark` (dark). The `.dnd-dark` class is applied by `DnDLogComponent` via the `IsDarkMode` cascading parameter.

## DamageTypeEnum → Color Token

Use `DamageTypeLogExtensions.ToLogColorToken()` to convert a `DamageTypeEnum` to the matching color token string:

```csharp
using DnDFightTool.Business.DnDActions.LogActions;

var colorToken = damageRoll.DamageType.ToLogColorToken(); // e.g., "fire"
var logContent = $"[c:{colorToken}][b]{damage}[/b] {damageRoll.DamageType}[/c]";
```

## Handler Patterns

### When to inject IDnDLogService

No handler injects `IDnDLogService` directly. Block/scope lifecycle is managed via sub-commands (`OpenBlockCommand`, `CloseBlockCommand`, `OpenScopeCommand`, `CloseScopeCommand`). Log entries are added via `WriteLogCommand`. Everything goes through `_mediator`.

Only `WriteLogCommandHandler`, `OpenBlockCommandHandler`, `CloseBlockCommandHandler`, `OpenScopeCommandHandler`, and `CloseScopeCommandHandler` inject `IDnDLogService` — they are the leaf handlers that call the service.

| Handler type | Injects IDnDLogService? | Pattern |
|---|---|---|
| Any orchestrating handler | ❌ No | Uses `OpenBlockCommand`/`WriteLogCommand`/etc. as sub-commands |
| Structure leaf handlers (Open/CloseBlock, Open/CloseScope) | ✅ Yes | Called only via sub-commands, never directly from business handlers |
| Content leaf handler (WriteLog) | ✅ Yes | Called only via sub-commands |

### Top-level orchestrator (opens a block with scopes)

```csharp
public class MyAttackCommandHandler : CommandHandlerBase<MyAttackCommand>
{
    private readonly IFightContext _fightContext;

    public MyAttackCommandHandler(IUndoableMediator mediator, IFightContext fightContext)
        : base(mediator)
    {
        _fightContext = fightContext;
    }

    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(MyAttackCommand command)
    {
        var caster = _fightContext[command.CasterId]!;
        var target = _fightContext[command.TargetId]!;

        await _mediator.SendAsSubCommandAsync(new OpenBlockCommand("Attack"), parentCommand: command);
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{caster.Name}[/b] attacks [b]{target.Name}[/b]"),
            parentCommand: command);

        await _mediator.SendAsSubCommandAsync(new OpenScopeCommand(), parentCommand: command);
        // ... detail entries ...
        await _mediator.SendAsSubCommandAsync(new CloseScopeCommand(), parentCommand: command);

        await _mediator.SendAsSubCommandAsync(new CloseBlockCommand(), parentCommand: command);

        return CommandResponse.Success();
    }
}
```

> **Undo behavior**: `WriteLogCommand` entries are hidden via their own undo. `OpenBlockCommand`/`CloseBlockCommand`/`OpenScopeCommand`/`CloseScopeCommand` are all no-ops on undo — indent level is baked into each entry at creation time, and `DnDLogComponent` automatically hides blocks with no visible entries.

### Leaf handler (single log entry, no block)

```csharp
public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(LooseHpCommand command)
{
    var fighter = _fightContext[command.TargetId] ?? throw new ArgumentException(...);
    var hitPoints = fighter.HitPoints;  // resolve once, reuse

    // ... mutate state via hitPoints ...

    await _mediator.SendAsSubCommandAsync(
        new WriteLogCommand($"[b]{fighter.Name}[/b] loses [b]{command.CorrectedAmount}[/b] HPs"),
        parentCommand: command);

    return CommandResponse.Success();
}
```

### Damage entry with color token

```csharp
var colorToken = damageRoll.DamageType.ToLogColorToken();
await _mediator.SendAsSubCommandAsync(
    new WriteLogCommand($"[c:{colorToken}][b]{effectiveDamage}[/b] {damageRoll.DamageType}[/c]"),
    parentCommand: command);
```

### Healing entry

```csharp
await _mediator.SendAsSubCommandAsync(
    new WriteLogCommand($"[b]{fighter.Name}[/b] regains [c:heal][b]{amount}[/b] HPs[/c]"),
    parentCommand: command);
```

## Key Files

| File | Purpose |
|------|---------|
| `src/Domain/Logs/IDnDLogService.cs` | Service interface |
| `src/Domain/Logs/DnDLogService.cs` | Implementation |
| `src/Domain/Logs/LogColorToken.cs` | Enum + extensions |
| `src/Domain/Logs/LogEntry.cs` | Entry record |
| `src/Domain/Logs/LogBlock.cs` | Block class |
| `src/Business/DnDActions/LogActions/WriteLog/` | Content sub-command + handler |
| `src/Business/DnDActions/LogActions/OpenBlock/` | Block-open sub-command + handler |
| `src/Business/DnDActions/LogActions/CloseBlock/` | Block-close sub-command + handler |
| `src/Business/DnDActions/LogActions/OpenScope/` | Scope-open sub-command + handler |
| `src/Business/DnDActions/LogActions/CloseScope/` | Scope-close sub-command + handler |
| `src/Business/DnDActions/LogActions/DamageTypeLogExtensions.cs` | DamageType → color token |
| `src/UI/FightBlazorComponents/Log/` | All UI components |
| `src/UI/FightBlazorComponents/Log/Parsing/` | Token parser |
| `src/Components/DndUi.Shared/wwwroot/css/log-colors.css` | Theme colors |
