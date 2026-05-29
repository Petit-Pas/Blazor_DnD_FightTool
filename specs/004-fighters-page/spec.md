# Feature Specification: Fighters Page

**Feature Branch**: `004-fighters-page`  
**Created**: 2026-04-28  
**Status**: Draft  
**Input**: User description: "I want a new page to add fighters to the fight. The current AddToFight button on the character sheet page disappears. Adding to fight becomes a command that can be undone. The page displays on the left all the things I can add to the fight, on the right all the things already in the fight. Adding to the fight prompts for initiativeRoll, except if it's a monster and there already is a monster (of the same kind) in the fight; in that case the initiative of the previous monster is copied. Removing from the fight manually is another command, not the undo of it, and that removing can be undone as well. The new page is called Fighters, while the fight page should be renamed FightDashboard. Adding/Removing is made with the click of a + or - button. Monsters and Characters should somehow be split (visually grouped). The initiative should come from a query that is prompted to the user through the new AddToFight command, so the page must expose its DialogService just like the existing FightPage does."

## Clarifications

### Session 2026-04-28

- Q: When adding a monster, the initiative prompt is skipped if "a monster is already in the fight". Which monsters share initiative? → A: Same kind only — only monsters originating from the same character template share initiative. Different monster types each get their own initiative roll. Note: this implies `FightingCharacter` must expose the original character id (currently only tracked inside `FightContext._monsterCountByOriginalId`).
- Q: Should clicking the "-" (remove) button require confirmation? → A: No confirmation — removal is immediate; recovery relies on undo.
- Q: How should the left-hand "addable" panel handle a player already in the fight? → A: Players already in the fight are filtered out of the left panel entirely (not shown at all). Monsters remain visible always (they can be added repeatedly).
- Q: What happens when the currently-active fighter is removed mid-fight? → A: Do not block, do not auto-advance. The active/selected fighter becomes `null`; FightDashboard handles a null selection gracefully (shows no selection). No turn-advancement logic is added by this feature (explicit non-goal).
- Q: When a monster is removed, should the per-template monster counter decrement? → A: Yes. Decrement on remove. If the count reaches 0, the next add of that monster template re-prompts for initiative; otherwise the new monster still inherits initiative from any remaining same-kind monster.
- Q: How should fighters be ordered in the right-hand panel? → A: Sort by `InitiativeRoll` descending (combat order). Ties keep stable insertion order.
- Q: Should adding a fighter auto-navigate to FightDashboard? → A: No auto-navigation. The user explicitly clicks FightDashboard from the nav.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Add a Character/Monster to the Fight (Priority: P1)

As a DM preparing or running a fight, I want a dedicated Fighters page where I can pick characters and monsters from a list and add them to the current fight with a single click, so that I can build the encounter without opening individual character sheets.

**Why this priority**: This is the core value of the feature — composing the fight roster. Without it, the rest of the page has nothing to operate on.

**Independent Test**: Open the Fighters page, click `+` next to a player and a monster, confirm both appear on the right-hand "in fight" list and that the player triggered an initiative prompt.

**Acceptance Scenarios**:

1. **Given** the Fighters page is open and the fight is empty, **When** the user clicks `+` next to a player character, **Then** an initiative prompt appears, the user provides an initiative roll, and the player is added to the right-hand list with that initiative.
2. **Given** a player has just been added to the fight, **When** the Fighters page re-renders, **Then** that player no longer appears in the left-hand "addable" list (filtered out entirely).
3. **Given** the fight is empty, **When** the user clicks `+` next to a monster template, **Then** an initiative prompt appears and the monster is added to the right-hand list with the provided initiative.
4. **Given** the fight already contains at least one monster of a given template, **When** the user clicks `+` next to that same monster template again, **Then** no initiative prompt appears and the new monster is added with the same initiative roll as the previously-added monster of that template.
5. **Given** the fight already contains a monster of a different template, **When** the user clicks `+` next to a new monster template, **Then** an initiative prompt does appear (different kind = independent initiative).
6. **Given** the user is prompted for initiative, **When** the user cancels the prompt, **Then** the character is not added to the fight.
7. **Given** any fighter has just been added, **When** the right-hand list re-renders, **Then** fighters are ordered by `InitiativeRoll` descending, ties preserving insertion order.

---

### User Story 2 - Remove a Fighter from the Fight (Priority: P1)

As a DM, I want to remove a fighter from the current fight using a `-` button, so that I can correct mistakes or react to fighters fleeing/dying without restarting the fight.

**Why this priority**: Equally essential as adding — a fight roster needs to be editable both ways.

**Independent Test**: With at least one fighter in the fight, click `-` next to it, confirm it disappears from the right-hand list immediately and is no longer present in the fight.

**Acceptance Scenarios**:

1. **Given** at least one fighter is in the fight, **When** the user clicks `-` next to a fighter on the right-hand list, **Then** the fighter is immediately removed (no confirmation dialog).
2. **Given** the user just removed a fighter, **When** the user triggers undo, **Then** the fighter is restored to the fight with its previous state (including initiative).
3. **Given** the removed fighter was a player, **When** the page re-renders, **Then** that player reappears in the left-hand "addable" list.
4. **Given** the removed fighter was a monster and was the last one of its template in the fight, **When** the user later clicks `+` on that same template, **Then** the initiative prompt appears again.
5. **Given** the removed fighter was the currently-active fighter on FightDashboard, **When** the removal completes, **Then** the active/selected fighter becomes `null` (no auto-advance, no block); FightDashboard renders gracefully with no selection.

---

### User Story 3 - Undoable Add/Remove (Priority: P1)

As a DM, I want both "add to fight" and "remove from fight" to be undoable through the existing undo/redo controls, so that misclicks can be corrected without manual cleanup.

**Why this priority**: The user explicitly requested both operations be undoable commands. Without this, the new page degrades the existing undo experience for fight composition.

**Independent Test**: Add a fighter, undo → fighter is gone. Remove a fighter, undo → fighter is back. Redo each → operation is reapplied.

**Acceptance Scenarios**:

1. **Given** the user added a fighter, **When** undo is triggered, **Then** the fighter is removed from the fight.
2. **Given** the user removed a fighter, **When** undo is triggered, **Then** the fighter is restored to the fight in its previous state.
3. **Given** an add or remove was undone, **When** redo is triggered, **Then** the operation is reapplied.
4. **Given** an "add monster" was a non-first-of-kind (no prompt, copied initiative), **When** undo is triggered, **Then** only that fighter is removed; the original monster of that kind remains.

---

### User Story 4 - Visual Grouping of Monsters and Characters (Priority: P2)

As a DM scanning a long roster, I want monsters and player characters to be visually grouped on both the available list and the in-fight list, so that I can quickly find what I'm looking for.

**Why this priority**: Quality-of-life improvement that materially affects usability with non-trivial rosters, but the page is functional without it.

**Independent Test**: Open the Fighters page with a mix of player and monster characters. Confirm players and monsters appear in distinct visual groups on both sides of the page.

**Acceptance Scenarios**:

1. **Given** the character repository contains both players and monsters, **When** the Fighters page renders, **Then** the left-hand list shows players and monsters in two visually distinct groups.
2. **Given** the fight contains both players and monsters, **When** the Fighters page renders, **Then** the right-hand list shows fighting players and fighting monsters in two visually distinct groups.

---

### User Story 5 - Rename Fight Page to FightDashboard (Priority: P2)

As a user navigating the app, I want the existing "Fight" page renamed to "FightDashboard" to clearly distinguish it from the new "Fighters" page used for composition.

**Why this priority**: Naming hygiene and navigation clarity. Independent of the rest of the feature; can ship after Stories 1–3 if needed.

**Independent Test**: Verify navigation entries: a "Fighters" entry leads to the new page, a "FightDashboard" entry leads to the previously-named "Fight" page; no orphan "Fight" entry remains.

**Acceptance Scenarios**:

1. **Given** the app is loaded, **When** the user opens navigation, **Then** they see entries for "Fighters" and "FightDashboard" (no entry simply called "Fight").
2. **Given** the user opens "FightDashboard", **Then** they see the same content/behavior previously found under "Fight".

---

### User Story 6 - Remove AddToFight Button from Character List Editor (Priority: P2)

As a DM, I want the `AddToFight` button removed from the character list editor page (where I edit/duplicate/delete characters) because the Fighters page is now the single place to compose a fight.

**Why this priority**: Removes UI duplication and an inconsistent (non-undoable) path into the fight, but is a cleanup that depends on Story 1 being available.

**Independent Test**: Open the character list editor; confirm no "Add to fight" button is shown next to player or monster entries.

**Acceptance Scenarios**:

1. **Given** the character list editor is open, **When** the user views any player or monster row, **Then** there is no "Add to fight" button.
2. **Given** the user wants to add to the fight, **When** they look for the action, **Then** it is only available on the Fighters page.

---

### Edge Cases

- Players already in the fight are NOT shown in the left-hand "addable" list at all (filtered out at the source). This eliminates the "add the same player twice" question entirely. Monsters are always shown on the left (repeatable adds).
- Adding a monster template repeatedly: each click produces a new fighter (numbered copy) — second and subsequent copies of the same template inherit initiative from the first without prompting.
- User cancels the initiative dialog: the add command is aborted; nothing is added; nothing is pushed to undo history.
- Undoing an add that triggered an initiative prompt: the fighter is removed and the monster-count for that template is decremented so a future add of the same template prompts again (since no monster of that kind remains).
- Removing a monster decrements the per-template counter. Removing the last monster of a kind, then adding another of that kind: the prompt MUST appear again (no leftover initiative to copy). If at least one same-kind monster remains, the next add still inherits initiative from a remaining same-kind monster.
- Removing the currently-active fighter (the one selected on FightDashboard) is permitted: the active/selected fighter becomes `null`. No turn auto-advance is performed; FightDashboard must render gracefully with a null selection. Re-selection is an explicit user action (out of scope of this feature).
- Empty character repository: the left-hand list shows an empty state message; right-hand list reflects current fight (possibly also empty).
- Navigation: adding/removing on the Fighters page does NOT auto-navigate to FightDashboard; the user explicitly navigates via the main nav.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The application MUST expose a new page named "Fighters" reachable from the main navigation.
- **FR-002**: The existing "Fight" page MUST be renamed to "FightDashboard" (route, navigation label, and any user-visible references).
- **FR-003**: The Fighters page MUST display, on the left, characters available to be added to the fight, visually grouped by `CharacterType`. Player characters already present in the current fight MUST be filtered out of this list entirely. Monster templates MUST always be shown (they can be added repeatedly).
- **FR-004**: The Fighters page MUST display, on the right, all fighters currently in the fight, visually grouped by `CharacterType`, sorted within each group by `InitiativeRoll` descending; ties MUST preserve stable insertion order.
- **FR-005**: Each entry in the left-hand list MUST expose a `+` button that, when clicked, dispatches an undoable `AddToFight` command for that character.
- **FR-006**: Each entry in the right-hand list MUST expose a `-` button that, when clicked, dispatches an undoable `RemoveFromFight` command for that fighter, with no confirmation dialog.
- **FR-007**: The `AddToFight` command MUST prompt the user for an initiative roll via a query (using the page's `DialogService`) UNLESS the character being added is a monster AND another monster originating from the same character template is already in the fight; in that case the new monster MUST inherit the previous monster's `InitiativeRoll` and no prompt is shown.
- **FR-008**: When the initiative prompt is cancelled, the `AddToFight` command MUST NOT mutate the fight state and MUST NOT be recorded in undo history.
- **FR-009**: The `AddToFight` command's undo MUST remove the added fighter and restore any internal monster bookkeeping (e.g., per-template counters) such that future adds behave as if the command never ran.
- **FR-010**: The `RemoveFromFight` command MUST be a separate command from the undo of `AddToFight`; manually removing a fighter is an explicit user action recorded in undo history.
- **FR-011**: The `RemoveFromFight` command's undo MUST restore the removed fighter with its prior state (including `InitiativeRoll`, current HP, statuses, etc.).
- **FR-011a**: When `RemoveFromFight` removes a monster, the per-template monster counter MUST be decremented. When the counter reaches 0, the next `AddToFight` of that template MUST re-prompt for initiative. While the counter is > 0, subsequent adds of that template MUST continue to inherit initiative from a remaining same-kind monster. Undo of `RemoveFromFight` MUST restore the counter.
- **FR-011b**: If the removed fighter is the currently-active/selected fighter on FightDashboard, the active selection MUST become `null`. The feature MUST NOT auto-advance the turn or block the removal. FightDashboard is expected to handle a null selection gracefully (rendering no selection); turn-advancement on removal is an explicit non-goal of this feature.
- **FR-012**: The Fighters page MUST set the global `IDialogServiceProvider` with its own `IDialogService` (mirroring `FightPage`'s pattern) so that command-driven queries can prompt the user.
- **FR-013**: The "Add to fight" button MUST be removed from the character list editor page; the Fighters page is the only entry point for composing the fight.
- **FR-014**: `FightingCharacter` (or the equivalent fight-side entity) MUST expose the originating character template id, so the `AddToFight` command can determine whether a monster of the same kind is already in the fight without depending on private state of `FightContext`.
- **FR-015**: Adding/removing fighters from the Fighters page MUST update both lists (left and right) reactively without a manual refresh, including: (a) re-filtering players out of the left list when added and back into it when removed, and (b) re-sorting the right list by `InitiativeRoll` descending.
- **FR-016**: Existing functionality of the FightDashboard (formerly Fight) page MUST remain unchanged in behavior aside from the rename and the requirement to render gracefully when the active/selected fighter is `null` (per FR-011b).
- **FR-017**: The Fighters page MUST NOT auto-navigate to FightDashboard after add or remove operations. Navigation between the two pages is exclusively user-initiated via the main navigation.

### Key Entities *(include if feature involves data)*

- **Character (template)**: Existing entity in the character repository. Has a `Type` (Player/Monster) and an `Id` used as the originating template id for monster grouping.
- **FightingCharacter**: Existing entity representing a participant in the current fight. Must additionally track the originating template `Id` (new, exposed publicly) to support same-kind initiative inheritance from outside `FightContext`.
- **AddToFight command**: New undoable command that takes a `Character` (template), resolves the initiative (prompt or inherit), and adds a `FightingCharacter` to the `FightContext`. Undo removes the fighter and reverts internal counters.
- **RemoveFromFight command**: New undoable command that takes a `FightingCharacter` (or its id), removes it from the `FightContext`, and on undo restores its full prior state.
- **Initiative query**: New user-interaction query that prompts for an initiative roll for a single character via the page's `DialogService`.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A DM can compose a full fight roster (players + several monsters of multiple kinds) entirely from the Fighters page, without opening any character sheet.
- **SC-002**: Adding a second monster of the same kind requires zero additional clicks beyond the `+` button (no initiative prompt).
- **SC-003**: Any single add or remove performed from the Fighters page can be reverted with one undo click and reapplied with one redo click, with the resulting fight state matching what would exist had the action been performed/skipped naturally.
- **SC-004**: There is exactly one entry point in the application for adding a character to the fight (the Fighters page), eliminating the prior duplication with the character list editor.
- **SC-005**: 100% of misclicks on the `-` button are recoverable through undo without data loss (initiative, HP, statuses preserved).

## Assumptions

- The existing undo/redo infrastructure (`UndoableMediator`, command base classes, undo/redo buttons from feature 003) is reused; this feature only adds two new commands and a query.
- The existing `IDialogServiceProvider`/`IDialogService` pattern used by `FightPage` is suitable and will be replicated on the Fighters page without architectural changes.
- The character repository (`ICharacterRepository`) and `IFightContext` APIs are sufficient sources of truth for the left and right lists respectively; no new persistence is introduced.
- "Same kind of monster" is defined as "shares the same originating `Character.Id`" (i.e., the same monster template). This matches existing `FightContext._monsterCountByOriginalId` semantics.
- The required exposure of the originating template id on `FightingCharacter` is in scope for this feature (per FR-014); it is treated as a small enabling change rather than a separate feature.
- Visual grouping of monsters vs. players is achieved via section headers/dividers within each list (left and right); exact styling is left to implementation.
- The route/name change from "Fight" to "FightDashboard" applies to the navigation label and the route path; deep links to `/fight` (if any) are out of scope for redirects unless trivially supported.
- No keyboard shortcuts are introduced for `+`/`-`; mouse/touch only.
- No bulk add/remove (e.g., "add 3 goblins at once") is in scope; each click adds exactly one fighter.
- Turn-advancement when removing the active fighter is explicitly out of scope; the feature only nulls the selection. Any future smart-advance behavior is a separate feature.
- The right-hand list ordering is presentation-only (sort by `InitiativeRoll` desc, stable on ties). The underlying `FightContext` storage order is not assumed to change.
