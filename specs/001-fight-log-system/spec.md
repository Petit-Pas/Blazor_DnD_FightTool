# Feature Specification: DnD Log System

**Feature Branch**: `001-fight-log-system`  
**Created**: 2026-04-24  
**Status**: Draft  
**Input**: User description: "In the fight page, there is a log placeholder. This log placeholder should not be replaced by its implementation. The logs must go through a very specific format (it's not the default ILogger thing). They should be created by the command handlers, and we should be able to: group them in a visual block, specify that within a scope next blocks are a bit more indented, through unique ids hide some logs, through unique ids show some logs again, through unique ids delete some logs for good. They should always keep their creation order, even if hidden then shown back."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Command Handler Emits Log Entries (Priority: P1)

As a user, when a command handler executes an action (e.g., a martial attack), the system produces log entries through the `IDnDLogService` that describe what happened, so that I can review the sequence of events in the log area.

**Why this priority**: Without log creation there is nothing to display. This is the foundational data-production story.

**Independent Test**: Execute any command handler and verify that log entries are created with unique IDs, preserving their creation order.

**Acceptance Scenarios**:

1. **Given** a command handler executes (e.g., ExecuteMartialAttackCommand), **When** the handler dispatches write-log sub-commands via UndoableMediator, **Then** one or more log entries are created with unique identifiers and a stable creation order.
2. **Given** a command handler dispatches sub-commands that also produce log entries, **When** all sub-commands complete, **Then** all entries retain their global creation order relative to the parent command's entries.
3. **Given** a command handler completes, **When** the log entries are inspected, **Then** each entry contains tokenized text with optional formatting tags (bold, color, hover).

---

### User Story 2 - Grouped Visual Blocks (Priority: P1)

As a user, I want related log entries to be visually grouped into a block (e.g., all entries from a single attack action), with clear visual spacing between blocks and a subtle highlight when I mouse over a block, so that I can quickly parse what happened in each action.

**Why this priority**: Grouping is the primary readability mechanism; without it the log is a flat, unreadable list.

**Independent Test**: Execute two separate commands and verify that each produces its own visually distinct, spaced block with hover highlighting.

**Acceptance Scenarios**:

1. **Given** a command handler explicitly opens and closes a named LogBlock via the `IDnDLogService` API, **When** entries are created between open and close, **Then** they appear inside a single visual block with a clear boundary.
2. **Given** two separate commands execute sequentially, **When** their log entries are rendered, **Then** each block is visually separated with spacing.
3. **Given** the user mouses over a block, **When** the cursor is within the block area, **Then** the entire block is lightly highlighted.
4. **Given** the cursor leaves the block area, **When** the highlight is inspected, **Then** it is removed.

---

### User Story 3 - Rich Text Formatting (Priority: P1)

As a user, I want log entries to display formatted text with bold emphasis, semantic colors (e.g., fire damage in a fire color, healing in a heal color), and hover tooltips, so that I can quickly scan important information at a glance.

**Why this priority**: Formatting is essential for readability — without it, all text looks the same and key values (damage amounts, damage types) are lost in a wall of text.

**Independent Test**: Create a log entry with formatting tokens and verify that the rendered output shows bold text, colored spans with the correct CSS variable color, and a tooltip on hover.

**Acceptance Scenarios**:

1. **Given** a log entry contains `[b]10[/b]`, **When** it is rendered, **Then** "10" appears in bold.
2. **Given** a log entry contains `[c:fire]fire damage[/c]`, **When** it is rendered, **Then** "fire damage" appears in the color defined by the `--log-color-fire` CSS custom property.
3. **Given** a log entry contains `[hover:Includes +2 modifier]18[/hover]`, **When** the user mouses over "18", **Then** a tooltip reading "Includes +2 modifier" appears, styled with the same color and font-weight as the "18" text (inheriting from the style at the start of the hovered section).
4. **Given** nested tags `[c:fire][b]10[/b] fire damage[/c]`, **When** rendered, **Then** "10" is bold and fire-colored, "fire damage" is fire-colored but not bold.
5. **Given** a hover span crosses a style boundary `[c:fire][hover:tooltip][b]10[/b] fire[/hover][/c]`, **When** the tooltip is shown, **Then** the tooltip inherits the style from the start of the hovered section (bold + fire color in this case).

---

### User Story 4 - Indented / Nested Scopes (Priority: P2)

As a user, I want certain log entries within a block to be visually indented (nested), so that I can see the hierarchical relationship between a parent action and its sub-actions (e.g., an attack roll followed by a damage roll).

**Why this priority**: Indentation communicates causality and hierarchy, making complex multi-step actions comprehensible.

**Independent Test**: Execute a command that dispatches sub-commands and verify that log entries from sub-commands are indented one level deeper than the parent entries.

**Acceptance Scenarios**:

1. **Given** a command handler opens an indentation scope, **When** subsequent log entries are created within that scope, **Then** those entries are rendered with increased indentation relative to the parent scope.
2. **Given** a nested scope is closed, **When** further log entries are created, **Then** those entries return to the previous indentation level.
3. **Given** multiple levels of nesting, **When** entries are rendered, **Then** each level is visually distinguishable by its indentation depth.

---

### User Story 5 - Undo Hides Log Entries, Redo Restores Them (Priority: P2)

As a user, when I undo a command, its associated log entries are automatically hidden (not deleted) from the log view, and when I redo that command, those entries reappear in their original positions, so that the log always reflects the current state of the fight.

**Why this priority**: Tight integration with undo/redo keeps the log accurate without requiring manual log management.

**Independent Test**: Execute a command that writes log entries, undo it, verify entries are hidden; redo it, verify entries reappear in their original order.

**Acceptance Scenarios**:

1. **Given** a command has been executed and produced log entries, **When** the command is undone, **Then** all log entries produced by that command (via write sub-commands) are hidden from the rendered view.
2. **Given** log entries have been hidden by an undo, **When** the command is redone, **Then** the entries reappear in their exact original creation-order positions.
3. **Given** entries A (from command 1), B, C (from command 2), D (from command 3) exist, **When** command 2 is undone, **Then** the log renders A, D — B and C are hidden but still exist internally.
4. **Given** command 2 has been undone (B, C hidden), **When** command 2 is redone, **Then** the log renders A, B, C, D in original order.
5. **Given** a log block has all its entries hidden by undo, **When** the block is inspected, **Then** the block itself is hidden. When at least one entry is redone, the block becomes visible again.

---

### User Story 6 - Semantic Color Tokens with Theme Support (Priority: P2)

As a user, I want damage types and healing to be displayed in distinct colors that adapt to light and dark themes, so that the log is readable and visually informative in any theme.

**Why this priority**: Color-coding damage types is a key visual aid for scanning log entries quickly.

**Independent Test**: Switch between light and dark themes and verify that all semantic color tokens render distinct, legible colors in both modes.

**Acceptance Scenarios**:

1. **Given** a log entry uses `[c:fire]`, **When** rendered in dark mode, **Then** the text color matches the `--log-color-fire` CSS variable defined for dark mode.
2. **Given** a log entry uses `[c:fire]`, **When** rendered in light mode, **Then** the text color matches the `--log-color-fire` CSS variable defined for light mode.
3. **Given** a log entry uses `[c:heal]`, **When** rendered, **Then** the text appears in the healing color, distinct from all damage type colors.
4. **Given** a log entry uses `[c:unknowntoken]` where `unknowntoken` is not defined in the `LogColorToken` enum, **When** rendered, **Then** the entire tag including its content is emitted as literal text (e.g. renders as `[c:unknowntoken]text[/c]`), making the authoring error visible.

---

### User Story 7 - Clear All Logs (Priority: P3)

As a user, I want to explicitly clear all log entries and blocks, so that I can start fresh (e.g., when beginning a new encounter).

**Why this priority**: Convenience feature — the log is a singleton that persists across fights, so users need a way to reset it.

**Independent Test**: Create log entries, invoke clear-all, verify the log is empty.

**Acceptance Scenarios**:

1. **Given** log entries and blocks exist, **When** a clear-all operation is invoked on `IDnDLogService`, **Then** all entries and blocks are permanently removed.
2. **Given** clear-all has been performed, **When** the log is inspected, **Then** it is empty — no entries, no blocks.

---

### User Story 8 - Retrofit All Existing Command Handlers with Logging (Priority: P3)

As a user, I want all existing command handlers to produce meaningful log entries so that every game action appears in the log from day one.

**Why this priority**: The log infrastructure and UI must exist first (P1/P2 stories); this story wires existing handlers into the system.

**Independent Test**: Execute each existing command handler and verify it produces log entries with proper blocks, scopes, and formatting.

**Acceptance Scenarios**:

1. **Given** the `IDnDLogService` and write-log sub-command infrastructure are available, **When** `ExecuteMartialAttackCommandHandler` executes, **Then** it produces a log block with entries describing the attack flow (hit roll, damage, status effects) using appropriate formatting tokens and scopes.
2. **Given** `ApplyDamageRollResultsCommandHandler` executes with multiple damage types, **When** log entries are created, **Then** each damage type value and its label are colored together using its semantic token (e.g., `[c:fire]6 fire damage[/c]`, `[c:cold]3 cold damage[/c]`).
3. **Given** `RegainHpCommandHandler` executes, **When** the log entry is created, **Then** the healed amount and its unit are wrapped together in the heal color token (e.g., `[c:heal]2 HPs[/c]`).
4. **Given** any of the 9 existing handlers executes, **When** the log is inspected, **Then** at least one log entry has been created.

**Handlers to retrofit** (9 total):

- `ExecuteMartialAttackCommandHandler`
- `ApplyDamageRollResultsCommandHandler`
- `TakeDamageCommandHandler`
- `LooseHpCommandHandler`
- `LooseTempHpCommandHandler`
- `RegainHpCommandHandler`
- `RegainTempHpCommandHandler`
- `ApplyStatusCommandHandler`
- `TryApplyStatusCommandHandler`

---

### User Story 9 - Developer Documentation: Instructions Update & Logging Skill (Priority: P3)

As a developer, I want the command handler instructions file (`.github/instructions/commands.instructions.md`) updated to include logging guidance, and a dedicated DnD-logging skill file created, so that any future command handler is implemented with proper logging from the start.

**Why this priority**: This is a documentation/tooling story that should be done last, after the implementation is finalized, so the skill reflects the actual API.

**Independent Test**: Read the updated instructions file and skill file; verify they contain correct, concrete guidance on using `IDnDLogService`, formatting tokens, blocks, scopes, and write-log sub-commands.

**Acceptance Scenarios**:

1. **Given** the logging system is fully implemented, **When** the commands instructions file is read, **Then** it contains a section explaining that handlers MUST produce log entries using write-log sub-commands.
2. **Given** the skill file exists at `.github/skills/dnd-logging/SKILL.md`, **When** a developer reads it, **Then** it contains: API reference for `IDnDLogService`, formatting tag reference (`[b]`, `[c:token]`, `[hover:text]`), semantic color token list, block/scope lifecycle patterns, and a complete handler example.
3. **Given** a new command handler is being implemented with Copilot, **When** the skill is referenced, **Then** the generated code includes proper log block open/close, log entries with formatting, and write-log sub-commands.

---

### Edge Cases

- What happens when undo targets a command that produced no log entries? No log visibility changes occur — undo proceeds normally for the command's other effects.
- What happens if a log block has zero visible entries after undo? The block itself is hidden (not rendered). It becomes visible again as soon as at least one of its entries is shown via redo.
- What happens with deeply nested indentation scopes? The system supports arbitrary nesting depth; the UI renders each level with incremental indentation.
- What happens with malformed formatting tokens (e.g., unclosed `[b]`)? The tag is rendered as literal text — no crash, no silent swallowing.
- What happens with an unknown semantic color token (not in `LogColorToken` enum)? The entire `[c:...]...[/c]` tag including its content is emitted as literal text — no color change, no crash, and the error is visible to the author.
- What happens when formatting tags are nested in an invalid order (e.g., `[b][c:fire][/b][/c]`)? Tags are matched by nearest open/close pair; overlapping tags result in the inner tag closing at its own `[/]` marker and the outer tag continuing.

## Requirements *(mandatory)*

### Functional Requirements

#### Log Data Model & Service

- **FR-001**: The system MUST provide a structured log mechanism (`IDnDLogService`) distinct from the standard `ILogger` infrastructure, purpose-built for game event logging.
- **FR-002**: Command handlers MUST be able to create log entries by dispatching write-log sub-commands through UndoableMediator during command execution.
- **FR-003**: Each log entry MUST be assigned a unique identifier (GUID) at creation time.
- **FR-004**: Log entries MUST maintain their creation order at all times, regardless of hide/show (undo/redo) operations.
- **FR-005**: A command handler MUST explicitly open and close a named `LogBlock` via the `IDnDLogService` API; entries created between open and close belong to that block.
- **FR-006**: The `IDnDLogService` API MUST support opening and closing named indentation scopes within an active block, causing subsequently created entries to render at an increased indentation level.

#### Undo / Redo Integration

- **FR-007**: Writing a log entry MUST be dispatched as a sub-command through UndoableMediator. Undoing this sub-command hides the entry (not deleted); redoing it restores visibility in original position.
- **FR-008**: The write-log sub-command MUST store the generated LogEntry unique ID back into the parent command object, so that the handler and subsequent undo/redo operations can reference it.
- **FR-009**: When all `LogEntry` items within a `LogBlock` are hidden (via undo), the block MUST also be treated as hidden. It becomes visible again as soon as at least one of its entries is shown (via redo).

#### Change Notification

- **FR-010**: `IDnDLogService` MUST expose a change-notification event on the interface itself (not only on the concrete class) so that UI components can subscribe without depending on the implementation.

#### Log Content Formatting

- **FR-011**: Log entry content MUST support inline formatting tokens using a BBCode-like tag syntax.
- **FR-012**: Supported formatting tags MUST include:
  - `[b]...[/b]` — bold text
  - `[c:token]...[/c]` — colored text, where `token` is a semantic name mapped to a CSS custom property `--log-color-{token}`
  - `[hover:tooltip text]...[/hover]` — mouseover tooltip on the wrapped section
- **FR-013**: Tags MUST support nesting (e.g., `[c:fire][b]10[/b] fire damage[/c]`).
- **FR-014**: The hover tooltip MUST inherit the style (color, font-weight) from the start of the hovered section. If styles change within the hover span, the tooltip uses the style active at the opening `[hover:]` tag.

#### Semantic Color Tokens

- **FR-015**: The system MUST define CSS custom properties (`--log-color-{token}`) for each of the following semantic tokens, with distinct values for light and dark themes:
  - Damage types: `bludgeoning`, `piercing`, `slashing`, `acid`, `cold`, `fire`, `force`, `lightning`, `necrotic`, `poison`, `psychic`, `radiant`, `thunder`
  - Silver and magic physical variants (`bludgeoning-silver`, `piercing-silver`, `slashing-silver`, `bludgeoning-magic`, `piercing-magic`, `slashing-magic`) MUST use the same color as their base type
  - `heal` — healing
- **FR-016**: If a `[c:token]` tag references a token that is not defined in the `LogColorToken` enum, the entire tag including its content MUST be emitted as literal text (e.g. `[c:invalid]text[/c]` renders as the string `[c:invalid]text[/c]`). This makes authoring errors visible rather than silently swallowing them.
- **FR-017**: The set of valid color tokens is defined by the `LogColorToken` enum. Only enum-defined tokens are accepted; the enum is the extension point for adding new tokens in the future.

#### Visual Presentation

- **FR-018**: Log blocks MUST be visually separated with spacing (margin/gap between blocks).
- **FR-019**: Mousing over a log block MUST lightly highlight the entire block (subtle background color change).
- **FR-020**: The log component MUST replace the existing "fight log placeholder" on the fight page.

#### Lifecycle

- **FR-021**: `IDnDLogService` MUST be registered as a singleton that persists for the application session.
- **FR-022**: `IDnDLogService` MUST provide an explicit clear-all operation that permanently removes all entries and blocks.

#### Existing Handler Retrofit

- **FR-023**: All 9 existing command handlers MUST be updated to produce log entries via write-log sub-commands, using appropriate blocks, scopes, and formatting tokens.
- **FR-024**: Damage-related handlers MUST wrap the damage value AND its type label together in the matching semantic color token (e.g., `[c:fire]6 fire damage[/c]`, not just `[c:fire]6[/c]`).
- **FR-025**: Healing-related handlers MUST wrap the healed value AND its unit together in the `[c:heal]` token (e.g., `[c:heal]2 HPs[/c]`, not just `[c:heal]2[/c]`).

#### Developer Documentation

- **FR-026**: The command handler instructions file (`.github/instructions/commands.instructions.md`) MUST be updated to include a logging section explaining that handlers produce log entries via write-log sub-commands.
- **FR-027**: A skill file MUST be created at `.github/skills/dnd-logging/SKILL.md` containing: `IDnDLogService` API reference, formatting tag syntax, semantic color token list, block/scope patterns, and a complete handler example. This MUST be the last implementation step, authored against the finalized code.

### Key Entities

- **LogEntry**: A single log item with a unique ID (GUID), tokenized text content with BBCode-like formatting tags (e.g., `"[c:fire][b]10[/b] fire damage[/c]"`), and indentation level. Entries are stored in creation order; there is no explicit order index. Visibility is NOT stored on the entry — the service tracks hidden entry IDs internally.
- **LogBlock**: A named group of LogEntry items, explicitly opened and closed by a command handler via the `IDnDLogService` API. Has a derived visibility state: hidden when all its entries are hidden; visible when at least one entry is visible.
- **LogScope**: A temporary context opened and closed by a command handler, causing subsequently created entries to render at an increased indentation level. Scopes can be nested.
- **IDnDLogService**: The service interface through which command handlers create, group, and manage log entries. Exposes a change-notification event directly on the interface. Registered as a singleton; persists across fights until explicitly cleared.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Every command handler execution that produces observable game events results in at least one log entry being created via a write sub-command.
- **SC-002**: Log entries hidden by undo and then shown by redo reappear in their exact original creation order position 100% of the time.
- **SC-003**: All semantic color tokens render legibly in both light and dark themes.
- **SC-004**: Users can visually distinguish grouped blocks (spacing + hover highlight) and indentation levels when the log is rendered.
- **SC-005**: Formatting tokens (bold, color, hover) render correctly in the log component, including nested combinations.
- **SC-006**: Malformed or unknown formatting tokens degrade gracefully — malformed tags and unknown color tokens render as literal text; no crashes, no invisible content.
- **SC-007**: All 9 existing command handlers produce at least one log entry when executed.
- **SC-008**: The commands instructions file and DnD-logging skill file exist and contain accurate, up-to-date guidance matching the implemented API.

## Assumptions

- The log service is a singleton that persists for the app session; it does NOT auto-clear on fight boundaries. Users clear it explicitly when desired.
- Command handlers already have access to injected services via constructor injection; `IDnDLogService` will follow the same injection pattern.
- Log entry content is tokenized text authored by the command handler using BBCode-like formatting tags — not raw exception traces or diagnostic data.
- Undo/redo of commands DOES affect log entries. Writing a log is a sub-command; undoing the parent cascades to its write sub-commands, hiding those entries. Redoing restores them. This is handled automatically by UndoableMediator's sub-command tree.
- The unique ID for each log entry is system-generated (GUID), not user-provided.
- The log component will be a Blazor component that subscribes to `IDnDLogService` change events and re-renders accordingly.
- CSS custom properties for semantic color tokens are defined in a shared `log-colors.css` stylesheet. Dark mode overrides are scoped to the `.mud-theme-dark` CSS class (applied by MudBlazor's theme cascade), not `@media (prefers-color-scheme)`, so they follow the app's manual theme toggle.

## Clarifications

### Session 2026-04-24

- Q: What format should log entry content take? → A: Tokenized text with BBCode-like formatting tags (`[b]`, `[c:token]`, `[hover:text]`). Originally plain text, upgraded to support rich formatting.
- Q: How does a LogBlock get started and ended? → A: Explicit — the command handler manually opens and closes the block via the `IDnDLogService` API.
- Q: How does the UI learn that the log service has changed? → A: Event/delegate exposed on the `IDnDLogService` interface; UI subscribes and calls `StateHasChanged`.
- Q: Who calls hide/show, and how? → A: Hide/show is not an explicit operation — it is a side effect of undo/redo. Writing a log is a sub-command; undoing it hides the entry, redoing it shows it.
- Q: Do hide/show apply to individual LogEntry items or also to LogBlock items? → A: Operations target individual LogEntry items via undo/redo; a LogBlock's visibility is derived automatically (hidden when all its entries are hidden, visible when at least one is visible).
- Q: Service naming and scope? → A: `IDnDLogService` — singleton, not fight-scoped. Persists across fights. Has explicit clear-all operation.
- Q: Is permanent delete in scope? → A: No — dropped. Undo/redo covers visibility. Clear-all is the only bulk removal.
- Q: Inline formatting syntax? → A: Custom BBCode-like tags: `[b]...[/b]`, `[c:token]...[/c]`, `[hover:text]...[/hover]`.
- Q: Color specification? → A: Semantic tokens mapped to CSS custom properties (`--log-color-{token}`). Defined by the `LogColorToken` enum (all damage types + physical variants + `heal`). Tokens not in the enum render as literal text. Both light and dark theme variants required.
