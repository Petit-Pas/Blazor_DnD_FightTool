# Research: Turn & Round Tracking

**Feature**: 002-turn-round-tracking  
**Date**: 2026-04-24

## R-001: Where should ICombatTurnService live?

**Decision**: New files in `src/Domain/Fight/TurnTracking/` (namespace `DnDFightTool.Domain.Fight.TurnTracking`). Registered as Singleton alongside `IFightContext`. NOT an extension of `IFightContext`.

**Rationale**:
- Turn tracking is fight-scoped — unlike `IDnDLogService` (session-scoped), it tracks state that belongs to the active fight.
- The constitution says "IFightContext is the single source of truth for the active fight session." However, `IFightContext` already has a focused responsibility (managing `FightingCharacter` identity, collection, and selection). Adding round/turn counters and ordered iteration to `IFightContext` would mix two orthogonal concerns.
- A dedicated `ICombatTurnService` gives it a clean API (state container, turn-order computation, events) without polluting `IFightContext`.
- Both remain Singletons registered in `Fight`'s `ServiceCollectionExtensions`. They are peers, not parent/child.
- No new project needed — `ICombatTurnService` and `CombatTurnService` live in `src/Domain/Fight/` which already exists.

**Alternatives considered**:
- Extend `IFightContext` → rejected: mixes character-management concerns with turn-progression concerns; makes `FightContext` significantly larger without clean separability.
- Put in a new project `Domain/CombatRules` → rejected: over-engineering; the concept is fight-scoped and doesn't justify a new project.
- Transient or Scoped lifetime → rejected: like `IFightContext`, this service must survive across component renders; Singleton is the correct lifetime.

---

## R-002: Log block lifecycle in the sub-command tree

**Decision**: `StartNextTurnCommand` dispatches `EndTurnCommand` (wraps `CloseBlockCommand`) and `StartTurnCommand` (wraps `OpenBlockCommand`) as sub-commands. `StartNextRoundCommand` logs the round change. Block re-opening on undo is handled automatically by `CloseBlockCommand.UndoAsync` via `ReopenBlock`.

**Rationale**:
- The turn advance has distinct phases: ending the current turn, optionally advancing the round, updating the active fighter, and starting the next turn. `EndTurnCommand` and `StartTurnCommand` encapsulate the log lifecycle for their respective phases.
- `EndTurnCommand` dispatches `CloseBlockCommand`. Future end-of-turn effects (status expiry, etc.) slot here.
- `StartTurnCommand` dispatches `OpenBlockCommand`. Future start-of-turn effects slot here.
- `StartNextRoundCommand` logs the round change (e.g., a round header entry).
- `StartNextTurnCommand` is the orchestrator: it stores state for undo, computes the next fighter, dispatches round changes, and updates service state.
- `LogBlock` already has a stable `Guid Id` — `ReopenBlock(Guid blockId)` sets `_currentBlock` back.
- `CloseBlockCommand` stores `ClosedBlockId` for `UndoAsync` → `ReopenBlock`.
- `OpenBlockCommandHandler.UndoAsync` calls `CloseBlock()` — reversing the opening.

**Required changes to `IDnDLogService`**: same as before.
- `CloseBlock()` return type: `void` → `Guid`. Returns the closed block's `Id`.
- Add `void ReopenBlock(Guid blockId)`.
- `OpenBlock(string name)` remains `void`.

**Alternatives considered**:
- Put `CloseBlockCommand`/`OpenBlockCommand` directly in `StartNextTurnCommand` (skip `EndTurnCommand`/`StartTurnCommand`) → rejected: flattens domain semantics; makes it harder to add future turn-lifecycle effects.
- Store the previous block name and re-open a new block with the same name → rejected: creates duplicate blocks; confuses the renderer.
- Store the previous `LogBlock` object in the command → rejected: commands must be serializable.

---

## R-003: Attack handler block-to-scope migration

**Decision**: Replace `OpenBlockCommand` / `CloseBlockCommand` in `ExecuteMartialAttackCommandHandler` with `OpenScopeCommand` / `CloseScopeCommand`. The attack no longer owns its own top-level log block — it runs as a scope within the currently-open turn block.

**Rationale**:
- The spec (FR-009) requires all attacks during a fighter's turn to appear indented beneath the turn heading, not as independent top-level sections.
- `OpenScopeCommand` and `CloseScopeCommand` already exist in `DnDActions/LogActions/`. Only the call sites in `ExecuteMartialAttackCommandHandler` need to change.
- The attack log's name becomes the content of a `WriteLogCommand` header entry (first entry in the scope at indent level N), not a block name. Alternatively, no header entry is needed — the scope's entries are self-explanatory.
- Undo of the attack (hiding its `WriteLogCommand` sub-commands) automatically collapses the scope to nothing visible, which is the correct behaviour.

**Impact scope**: Only `ExecuteMartialAttackCommandHandler.OpenAttackLog()` and `CloseAttackLog()` methods. No other command handler currently uses `OpenBlockCommand` / `CloseBlockCommand`.

**Alternatives considered**:
- Keep blocks for attacks AND add turn blocks as a parent → rejected: the spec explicitly says attacks become scopes, not blocks.
- Remove `OpenBlockCommand` / `CloseBlockCommand` from the codebase entirely → deferred: they will still be needed for future scenarios (e.g., opening a block for a special ability). Keep them; just don't use them in the attack handler.

---

## R-004: StartNextTurnCommand state and service role

**Decision**: `StartNextTurnCommand` is the main orchestrator. It stores `PreviousFighterId: Guid?` for undo. `ICombatTurnService` is a pure state container — no `Advance()`/`Revert()` methods. The command reads state from the service, computes what happens next, mutates the service via setters, and dispatches sub-commands. A new `StartNextRoundCommand` handles round transitions.

**Rationale**:
- The service holds turn order, current fighter, round number, and fires events for UI binding. It does not drive state transitions — the commands do.
- `StartNextTurnCommand` stores `PreviousFighterId` (captured before mutation) so `UndoAsync` can restore the previous fighter or clear the selection if it was the first turn.
- `StartNextRoundCommand` stores `PreviousRound` so `UndoAsync` can restore the round number.
- No internal undo stack on the service — each command owns its own undo data. This keeps the service a simple state container.
- `RedoAsync` on `StartNextTurnCommand` clears sub-commands and re-executes (standard redo pattern).

**Alternatives considered**:
- Service owns `Advance()`/`Revert()` with internal undo stack → rejected: makes the service a state machine when it should be a state container; the commands already have undo infrastructure via UndoableMediator.
- Separate `StartCombatCommand` and `NextTurnCommand` → rejected: same button, same logic; a single `StartNextTurnCommand` handles both via `service.IsStarted`.

---

## R-005: CombatStatusComponent placement and button state

**Decision**: New Blazor component `CombatStatusComponent` in `src/UI/FightBlazorComponents/CombatStatus/`. It replaces the `<div style="grid-row:2; grid-column:2;">general fight infos</div>` placeholder in `FightPage.razor`. The button is a `<MudButton>` whose label the component computes from `ICombatTurnService.IsStarted`, disabled until at least one fighter is in `IFightContext`.

**Rationale**:
- The constitution places fight-screen entity components in `src/UI/FightBlazorComponents/Entities/{Feature}/`, but `CombatStatusComponent` is not tied to a specific entity — it represents the state of the fight as a whole. It belongs at the same level as `Log/`, as a feature-level folder directly under `FightBlazorComponents/`.
- The component subscribes to both `ICombatTurnService.OnChanged` and `IFightContext.OnActiveFighterChanged` to keep the display current.
- The button label (`"Start Combat"` vs `"Next Turn"`) is exposed by the service via `IsStarted` — the component computes the label in the code-behind.
- Disabling the button when no fighters exist prevents sending `StartNextTurnCommand` against an empty fight.

**Alternatives considered**:
- Inline the button in `FightPage.razor` → rejected: logic in razor files is prohibited by the constitution.
- A separate `RoundTrackerComponent` + `TurnButtonComponent` → rejected: over-splitting for a small UI area; single component with code-behind is sufficient.
