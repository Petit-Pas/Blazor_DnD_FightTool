# Feature Specification: Undo/Redo Buttons on Fight Page

**Feature Branch**: `003-undo-redo-buttons`  
**Created**: 2026-04-27  
**Status**: Draft  
**Input**: User description: "Add the ability to undo/redo commands from the fight page. The buttons should be located in the component that displays rounds and turns (bottom right of the fight page). Two simple buttons with left and right curved arrows (like circular undo/redo icons). Each button individually greys out when there is nothing to undo/redo respectively."

## Clarifications

### Session 2026-04-27

- Q: How should the buttons detect undo/redo history changes after non-turn commands? → A: Subscribe to `OnCommandExecuted`, `OnCommandUndone`, and `OnCommandRedone` events on `IUndoableMediator` (library will be updated to expose these).
- Q: Where within the combat status panel should the undo/redo buttons be placed? → A: Statically at the top-right corner of the panel.
- Q: Should undo/redo buttons be visible when no combat is active? → A: Always visible; enabled/disabled based purely on history length.
- Q: Can the user undo StartCombat itself (reverting to pre-combat state)? → A: Yes — full history, no boundary.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Undo Last Action (Priority: P1)

As a DM running a fight, I want to undo the last action I performed so that I can correct mistakes without restarting the encounter.

**Why this priority**: Undo is the core value of this feature — it lets the DM recover from misclicks or incorrect inputs immediately.

**Independent Test**: Can be fully tested by performing any combat action (e.g., advancing a turn) and clicking the undo button, then verifying the game state reverts.

**Acceptance Scenarios**:

1. **Given** at least one command has been executed in the current session, **When** the user clicks the undo button, **Then** the last command is undone and the game state reverts to its previous state.
2. **Given** no commands have been executed (or all have been undone), **When** the user views the undo button, **Then** the button appears greyed out and is not clickable.
3. **Given** multiple commands have been executed, **When** the user clicks undo repeatedly, **Then** commands are undone one at a time in reverse order.

---

### User Story 2 - Redo Undone Action (Priority: P2)

As a DM who just undid an action, I want to redo it so that I can restore it if the undo was accidental.

**Why this priority**: Redo complements undo and prevents the user from being stuck after an accidental undo.

**Independent Test**: Can be fully tested by performing an action, undoing it, then clicking redo, and verifying the game state is restored.

**Acceptance Scenarios**:

1. **Given** at least one command has been undone, **When** the user clicks the redo button, **Then** the last undone command is re-applied and the game state updates accordingly.
2. **Given** no commands have been undone (or all have been redone), **When** the user views the redo button, **Then** the button appears greyed out and is not clickable.
3. **Given** multiple commands have been undone, **When** the user clicks redo repeatedly, **Then** commands are re-applied one at a time in the original execution order.

---

### User Story 3 - Visual Feedback of Undo/Redo Availability (Priority: P3)

As a DM, I want to immediately see whether undo or redo is available so that I know my options without guessing.

**Why this priority**: Clear visual state reduces cognitive load and prevents confusion during fast-paced combat.

**Independent Test**: Can be tested by observing button states across different sequences of actions, undos, and redos.

**Acceptance Scenarios**:

1. **Given** a fresh session with no actions taken, **When** the fight page loads, **Then** both undo and redo buttons are greyed out.
2. **Given** a command was just executed (new action after previous undos), **When** the user views the buttons, **Then** the undo button is active and the redo button is greyed out (redo history cleared by new action).
3. **Given** an action was undone, **When** the user views the buttons, **Then** undo remains active (if more history exists) and redo becomes active.

---

### Edge Cases

- What happens when the user rapidly clicks undo/redo multiple times in quick succession? The buttons should reflect the current state after each operation completes.
- What happens when undo/redo fails (e.g., returns `false`)? The button state should remain consistent with the actual history lengths.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The fight page MUST display an undo button and a redo button within the combat status panel, positioned statically at the top-right corner of the panel.
- **FR-002**: The undo button MUST trigger undo of the last executed command when clicked.
- **FR-003**: The redo button MUST trigger redo of the last undone command when clicked.
- **FR-004**: The undo button MUST be disabled (greyed out) when there are no commands to undo.
- **FR-005**: The redo button MUST be disabled (greyed out) when there are no commands to redo.
- **FR-006**: Both buttons MUST update their enabled/disabled state by subscribing to `OnCommandExecuted`, `OnCommandUndone`, and `OnCommandRedone` events on `IUndoableMediator`.
- **FR-007**: The undo button MUST display a left-curved arrow icon (standard circular undo icon).
- **FR-008**: The redo button MUST display a right-curved arrow icon (standard circular redo icon).
- **FR-009**: The buttons MUST NOT interfere with the existing combat status functionality (round display, turn display, next turn button).
- **FR-010**: The buttons MUST be visible at all times (regardless of combat state), with enabled/disabled driven solely by history length.
- **FR-011**: The undo button MUST be able to undo any command in the mediator history, including StartCombat (no scope boundary).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can undo any fight action with a single click from the fight page.
- **SC-002**: Users can redo any undone action with a single click from the fight page.
- **SC-003**: Button disabled state accurately reflects availability 100% of the time (no stale state).
- **SC-004**: The undo/redo buttons are discoverable without instruction — recognizable icons in a logical location.

## Assumptions

- The existing undo/redo infrastructure (UndoableMediator) already handles all command reversal logic; this feature only adds UI triggers.
- The UndoableMediator library will be updated to expose `OnCommandExecuted`, `OnCommandUndone`, and `OnCommandRedone` events on `IUndoableMediator` (prerequisite for this feature).
- The undo/redo buttons apply to all commands dispatched through the mediator, not just turn-related commands. There is no scope boundary — even StartCombat can be undone.
- No keyboard shortcuts are included in this feature (buttons only).
- No confirmation dialogs are needed before undo/redo — the action is immediate.
- The redo history is cleared when a new command is executed after an undo (standard redo behavior, already handled by the mediator).
