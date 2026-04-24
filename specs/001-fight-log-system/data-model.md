# Data Model: DnD Log System

**Feature**: 001-fight-log-system  
**Date**: 2026-04-24

## Entities

### LogEntry

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | Unique identifier, system-generated at creation time |
| Content | `string` | Tokenized text with BBCode-like formatting tags (e.g., `"[c:fire][b]10[/b] fire damage[/c]"`) |
| IndentLevel | `int` | Indentation depth at creation time (0 = root, incremented by scopes) |

**Notes**:
- `Content` must not be null or empty.
- `IndentLevel` must be >= 0.
- Entries are stored in creation order within their `LogBlock.Entries` list — no explicit `Order` property needed.
- Visibility is NOT stored on the entry. The service tracks hidden entry IDs internally (`HashSet<Guid>`). This keeps `LogEntry` a simple, immutable record that doesn't leak into command state.

---

### LogBlock

| Field | Type | Description |
|-------|------|-------------|
| Id | `Guid` | Unique identifier, system-generated at creation time |
| Name | `string` | Display name (e.g., `"Martial Attack"`) |
| Entries | `IReadOnlyList<LogEntry>` | Ordered list of entries in this block |

**Derived state**:
- `IsVisible`: `true` if at least one entry is not in the service's hidden set; `false` if all entries are hidden.
- Computed via `Entries.Any(e => !logService.IsHidden(e.Id))`

---

### IDnDLogService (interface)

```
Properties:
  IReadOnlyList<LogBlock> Blocks        — All blocks in creation order

Events:
  event Action? OnChanged               — Fired after any mutation (add entry, hide, show, clear)

Methods:
  void OpenBlock(string name)           — Start a new named block; subsequent entries belong to it
  void CloseBlock()                     — Close the current block
  void OpenScope()                      — Increase indent level for subsequent entries
  void CloseScope()                     — Decrease indent level
  Guid AddEntry(string content)         — Create a new LogEntry in the current block at current indent; returns the entry's GUID
  void Hide(Guid id)                    — Mark an entry as hidden (idempotent, no-op if already hidden or unknown ID)
  void Show(Guid id)                    — Mark an entry as visible (idempotent, no-op if already visible or unknown ID)
  bool IsHidden(Guid id)                — Check if an entry is currently hidden
  void Clear()                          — Remove all entries, blocks, and hidden state permanently
```

**Invariants**:
- `OpenBlock` / `CloseBlock` must be balanced. Calling `AddEntry` without an open block throws.
- `OpenScope` / `CloseScope` must be balanced within a block. Scope state resets on `CloseBlock`.
- `OnChanged` fires after every state mutation.
- Hidden state is tracked internally via a `HashSet<Guid>` — not on the `LogEntry` entity.

---

### WriteLogCommand (sub-command)

| Field | Type | Description |
|-------|------|-------------|
| Content | `string` | The tokenized text to write |
| LogEntryId | `Guid?` | `null` before execution; set to the created `LogEntry.Id` after execution |

**Behavior**:
- `ExecuteAsync`: calls `IDnDLogService.AddEntry(Content)`, stores returned GUID in `LogEntryId`.
- `UndoAsync`: calls `IDnDLogService.Hide(LogEntryId!.Value)`.
- `RedoAsync`: calls `IDnDLogService.Show(LogEntryId!.Value)`.

---

## Relationships

```
IDnDLogService 1──* LogBlock 1──* LogEntry
IDnDLogService ── HashSet<Guid> (hidden entry IDs)

WriteLogCommand ──> IDnDLogService.AddEntry()
WriteLogCommand ──> IDnDLogService.Hide() / .Show() [undo/redo]

CommandHandler ──SendAsSubCommandAsync──> WriteLogCommand
CommandHandler ──direct call──> IDnDLogService.OpenBlock/CloseBlock/OpenScope/CloseScope
```

## LogColorToken Enum (Domain layer)

```csharp
public enum LogColorToken
{
    Bludgeoning,
    Piercing,
    Slashing,
    BludgeoningSilver,
    PiercingSilver,
    SlashingSilver,
    BludgeoningMagic,
    PiercingMagic,
    SlashingMagic,
    Acid,
    Cold,
    Fire,
    Force,
    Lightning,
    Necrotic,
    Poison,
    Psychic,
    Radiant,
    Thunder,
    Heal
}
```

**Convention**: Enum value → CSS variable name via kebab-case conversion: `LogColorToken.BludgeoningSilver` → `--log-color-bludgeoning-silver`.

**Parser behavior**: The `[c:token]` tag's token string is matched against the enum (case-insensitive). If the token is not a valid `LogColorToken` value, the entire tag (including content) is emitted as literal text — e.g., `[c:invalid]text[/c]` renders as the string `[c:invalid]text[/c]`.

## BBCode Token Model (Presentation layer)

### LogToken (discriminated union / class hierarchy)

| Variant | Fields | Description |
|---------|--------|-------------|
| `TextToken` | `string Text` | Plain text segment |
| `BoldStart` | — | Opens bold span |
| `BoldEnd` | — | Closes bold span |
| `ColorStart` | `LogColorToken Token` | Opens colored span with validated semantic token |
| `ColorEnd` | — | Closes colored span |
| `HoverStart` | `string TooltipText` | Opens hover span with tooltip |
| `HoverEnd` | — | Closes hover span |

**Parser**: `LogTokenParser.Parse(string content) → IReadOnlyList<LogToken>`
- Single-pass scan; `[` starts a tag, `]` ends it.
- Unknown or malformed tags emitted as `TextToken` (graceful degradation).

## CSS Custom Properties

### File: `log-colors.css`

```css
/* Light mode (default) */
:root {
  --log-color-bludgeoning: #8B7355;
  --log-color-piercing: #8B7355;
  --log-color-slashing: #8B7355;
  --log-color-bludgeoning-silver: var(--log-color-bludgeoning);
  --log-color-piercing-silver: var(--log-color-piercing);
  --log-color-slashing-silver: var(--log-color-slashing);
  --log-color-bludgeoning-magic: var(--log-color-bludgeoning);
  --log-color-piercing-magic: var(--log-color-piercing);
  --log-color-slashing-magic: var(--log-color-slashing);
  --log-color-acid: #7FBA00;
  --log-color-cold: #4FC3F7;
  --log-color-fire: #E65100;
  --log-color-force: #9C27B0;
  --log-color-lightning: #1565C0;
  --log-color-necrotic: #4E342E;
  --log-color-poison: #558B2F;
  --log-color-psychic: #AD1457;
  --log-color-radiant: #F9A825;
  --log-color-thunder: #5C6BC0;
  --log-color-heal: #2E7D32;
}

/* Dark mode */
.mud-theme-dark {
  --log-color-bludgeoning: #D7CCC8;
  --log-color-piercing: #D7CCC8;
  --log-color-slashing: #D7CCC8;
  /* silver/magic variants inherit via var() */
  --log-color-acid: #C5E1A5;
  --log-color-cold: #81D4FA;
  --log-color-fire: #FF8A65;
  --log-color-force: #CE93D8;
  --log-color-lightning: #64B5F6;
  --log-color-necrotic: #A1887F;
  --log-color-poison: #AED581;
  --log-color-psychic: #F48FB1;
  --log-color-radiant: #FFF176;
  --log-color-thunder: #9FA8DA;
  --log-color-heal: #81C784;
}
```

**Fallback**: If a `[c:token]` tag references a token not in the `LogColorToken` enum, the entire tag is emitted as literal text. Components use `color: var(--log-color-{token}, inherit)` for valid tokens, so any missing CSS variable falls back gracefully to the parent text color.
