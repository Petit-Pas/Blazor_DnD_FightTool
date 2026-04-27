# Feature Specification: Turn & Round Tracking

**Feature Branch**: `002-turn-round-track`  
**Created**: 2026-04-24  
**Status**: Draft  
**Input**: User description: "During a fight, combat must follow a structured turn-based sequence. A round represents one full cycle where every fighter acts once. A turn belongs to a single fighter. The bottom-right of the fight screen shows the current round number and whose turn it is, along with a button to advance to the next turn. When a fighter's turn begins, they are automatically highlighted in the interface. Each fighter's initiative is computed automatically from their character data and determines the order in which they act each round. Advancing a turn should be reversible. In the fight log, each fighter's turn is the top-level grouping; all actions taken during that turn (attacks, etc.) appear as entries within it."

## Clarifications

### Session 2026-04-24

- Q: How does the game master enter initiative values for each fighter? → A: Not entered manually. Initiative is computed automatically from each fighter's character data via their existing initiative total; no GM input required.
- Q: What type of value is initiative? → A: Plain positive integer, as returned by the fighter's initiative computation.
- Q: What triggers the start of combat — Round 1, Turn 1? → A: The same "Next Turn" button, which is labeled "Start Combat" on the very first press only, then switches to "Next Turn" for all subsequent presses.
- Q: When a turn advancement is undone, what happens to log entries from the undone turn? → A: They are hidden, consistent with how other undone commands are handled in the log.
- Q: When a fighter is removed mid-round, are they skipped from the turn order? → A: No — they still take their turn as normal in the current and all future rounds.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Advance to the Next Fighter's Turn (Priority: P1)

As a game master running a fight, when the current fighter finishes their actions, I press "Next Turn" to hand control to the next fighter in order, so that the fight proceeds in a structured, fair sequence.

**Why this priority**: This is the core mechanic. Without the ability to advance turns, all other features are unreachable.

**Independent Test**: Start a fight with at least two fighters, press "Next Turn", and verify that the current turn fighter changes to the next one in order and that fighter becomes highlighted in the UI.

**Acceptance Scenarios**:

1. **Given** a fight is in progress with at least two fighters and Fighter A's turn is active, **When** the user presses "Next Turn", **Then** Fighter B becomes the current turn fighter, Fighter B is highlighted in the UI, and the current turn indicator updates.
2. **Given** Fighter A is the last fighter in a round, **When** the user presses "Next Turn", **Then** the round counter increments by 1, the first fighter in the order becomes active, and their turn is opened in the log.
3. **Given** a fight exists but no turns have started yet, **When** the fight screen is displayed, **Then** the button in the bottom-right is labeled "Start Combat".
4. **Given** the user presses "Start Combat" (the first press), **When** the action completes, **Then** Round 1 begins, the first fighter in initiative order becomes active, and the button is relabeled "Next Turn" for all subsequent presses.
5. **Given** no fight is active, **When** the fight screen is loaded, **Then** the button is disabled or not visible.

---

### User Story 2 - Display Current Round and Turn Information (Priority: P1)

As a game master, I want to always see which round the fight is in and whose turn it currently is, displayed in the bottom-right area of the fight screen, so that I can track the flow of combat at a glance.

**Why this priority**: Without a visible indicator, the game master loses track of the fight state. This directly supports Story 1 in terms of usability.

**Independent Test**: Start a fight, observe the bottom-right area shows "Round 1 — [Fighter Name]'s turn", advance a turn, and verify the display updates correctly.

**Acceptance Scenarios**:

1. **Given** a fight starts, **When** the fight screen is displayed, **Then** the bottom-right shows "Round 1" and the name of the first fighter whose turn it is.
2. **Given** the user presses "Next Turn", **When** the turn advances to the next fighter, **Then** the round/turn display updates to reflect the new current turn fighter.
3. **Given** the last fighter's turn ends, **When** the round increments, **Then** the round counter in the display increments and shows the first fighter of the new round.

---

### User Story 3 - Current Turn Fighter Auto-Highlighted in UI (Priority: P1)

As a game master, when a fighter's turn begins, that fighter is automatically highlighted/selected in the fight interface, so that I immediately know who is acting without having to find them manually.

**Why this priority**: Reduces cognitive load during combat; prevents "who's active?" confusion that would slow the game down.

**Independent Test**: Advance through two turns and verify that the fighter card/row for the current turn fighter is in a highlighted state each time.

**Acceptance Scenarios**:

1. **Given** the fight starts, **When** the first fighter's turn begins, **Then** that fighter is automatically visually highlighted in the fight interface.
2. **Given** Fighter A is active and "Next Turn" is pressed, **When** Fighter B's turn starts, **Then** Fighter B is selected and Fighter A's selection is cleared.
3. **Given** a new round begins, **When** the first fighter's turn starts again, **Then** that fighter is automatically re-selected in the UI.

---

### User Story 4 - Character Turn as Log Grouping (Priority: P2)

As a game master, I want the fight log to organize entries by fighter turn — each turn is headed by the fighter's name (e.g., "Gandalf's turn") and all actions taken during that turn appear nested beneath it — so that the log clearly reflects the flow of combat.

**Why this priority**: This redefines the log's top-level structure and makes it far easier to read during and after a fight.

**Independent Test**: Start a fight, execute one attack during Fighter A's turn, advance to Fighter B's turn, execute one attack, then verify the log shows two distinct turn sections, one per fighter, each with their respective actions nested inside.

**Acceptance Scenarios**:

1. **Given** Fighter A's turn begins, **When** the turn opens, **Then** the log shows a new top-level heading "[Fighter A]'s turn".
2. **Given** it is Fighter A's turn, **When** an attack action is executed, **Then** the attack entries appear indented beneath the "Fighter A's turn" heading, not as a separate top-level section.
3. **Given** Fighter A's turn ends (Next Turn pressed), **When** the turn section is closed, **Then** no further entries are added to it.
4. **Given** two fighters each take a turn, **When** the log is reviewed, **Then** the log shows two visually distinct top-level sections, one per fighter's turn, each containing their respective actions indented beneath.

---

### Edge Cases

- What happens if a fighter is removed (killed/fled) mid-round before their turn? Their turn still occurs as normal; the fighter remains in the turn order for the current and all future rounds.
- What happens if all fighters are removed before the round ends? The fight ends and the round/turn tracker stops.
- What happens if a fight has only one fighter? That fighter's turn starts and ends each round indefinitely.
- What happens if the user undoes a turn advancement? The previous fighter's turn resumes, their turn section in the log becomes active, and any subsequent entries are added to it. The UI highlights the previous fighter.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST track which fighter's turn it currently is within a fight.
- **FR-002**: The system MUST track the current round number, incrementing when all fighters in the current round have taken a turn.
- **FR-003**: The fight screen MUST display the current round number and current turn fighter's name in the bottom-right area at all times during a fight.
- **FR-004**: The fight screen MUST provide a button in the bottom-right area to advance combat. This button is labeled "Start Combat" before the first turn of the fight begins, and "Next Turn" for all subsequent presses.
- **FR-005**: When a fighter's turn begins, that fighter MUST be automatically highlighted/selected in the fight interface (UI state, not domain state).
- **FR-006**: When "Next Turn" is pressed, the system MUST advance to the next fighter according to the established turn order.
- **FR-007**: When the last fighter in a round completes their turn, pressing "Next Turn" MUST increment the round counter and return to the first fighter in order.
- **FR-008**: When a fighter's turn begins, the fight log MUST open a new top-level block named "[Fighter Name]'s turn".
- **FR-009**: All attacks and actions performed during a fighter's turn MUST appear as indented entries beneath that fighter's turn heading in the log, not as independent top-level sections.
- **FR-010**: When a fighter's turn ends (Next Turn pressed), the log MUST stop adding entries to that fighter's turn section.
- **FR-011**: Turn order within each round MUST be determined by each fighter's initiative total, computed automatically from their character data. Fighters act in descending initiative order (highest first). Ties are broken by the order in which fighters were added to the fight. No manual initiative entry by the game master is required.

### Key Entities

- **Round**: A numbered iteration of combat in which all fighters each take one turn. Increments when the last fighter's turn ends.
- **Turn**: One fighter's active period within a round. Has a start and an end. Associated with exactly one fighter.
- **Initiative**: A numeric value derived automatically from a fighter's character data that determines the order in which they act each round. Higher values act first; ties are broken by insertion order.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A game master can advance through a full round of combat for 4 fighters in under 10 seconds of interaction.
- **SC-002**: The current turn fighter changes and the UI highlights the new fighter with no perceptible delay after pressing "Next Turn".
- **SC-003**: After any number of turns, the round counter correctly reflects the number of complete rounds elapsed.
- **SC-004**: The fight log always shows entries grouped by fighter turn, with no attack entries appearing outside a turn block.
- **SC-005**: The correct fighter is selected in the UI 100% of the time when their turn begins — no manual selection is needed.

## Assumptions

- Turn order is derived automatically from each fighter's initiative total at the start of combat; no manual ordering step is required and reordering mid-fight is out of scope for this feature.
- Each fighter takes exactly one turn per round; there is no "extra turn" or "delay turn" mechanic in this version.
- The "Next Turn" button triggers an advance to the next turn immediately, with no confirmation dialog.
- A fight must have at least one fighter for the round/turn tracker to start.
- Advancing a turn can be undone; the undo/redo system already present in the fight screen applies to turn advancement.
- Mobile / touch interaction is handled by the existing UI framework and does not require special treatment here.
