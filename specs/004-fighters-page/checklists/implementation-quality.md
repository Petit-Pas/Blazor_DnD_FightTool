# Implementation Quality Checklist: Fighters Page

**Purpose**: Validate that requirements covering the highest-risk areas of the Fighters page feature are complete, clear, consistent, and measurable. This is a "unit test" of the spec / data-model — not of the implementation.
**Created**: 2026-04-28
**Feature**: [spec.md](../spec.md), [data-model.md](../data-model.md)

## Undoable Command Correctness

- [ ] CHK001 Are pre-mutation cancel semantics for `AddToFightCommand` explicit (no state change, no history entry) and consistently described between FR-008 and the data-model algorithm? [Consistency, Spec §FR-008, data-model §AddToFightCommandHandler]
- [ ] CHK002 Are the exact pieces of state that `AddToFightCommand` must capture for undo enumerated (added fighter id, resolved initiative, inherited flag)? [Completeness, data-model §AddToFightCommand]
- [ ] CHK003 Is the redo strategy for `AddToFightCommand` (clear sub-commands + re-execute) specified, including expected behavior when the user cancels the prompt during redo? [Clarity, data-model §AddToFightCommandHandler]
- [ ] CHK004 Are the exact pieces of state that `RemoveFromFightCommand` must capture for undo enumerated (full `FightingCharacter` instance including HP, statuses, initiative)? [Completeness, Spec §FR-011, data-model §RemoveFromFightCommand]
- [ ] CHK005 Is the ordering of sub-commands relative to the parent mutation (e.g., `SetCurrentFighterCommand` before `FightContext.Remove`, `WriteLogCommand` after) specified deterministically for both Execute and Undo cascade? [Clarity, data-model §RemoveFromFightCommandHandler]
- [ ] CHK006 Are failure / no-op paths for both commands defined (source character missing, fighter id missing on redo)? [Coverage, Edge Case]
- [ ] CHK007 Is it specified that `RemoveFromFight` is a *separate* command from the undo of `AddToFight`, including what user-visible difference (history entry presence) this implies? [Clarity, Spec §FR-010]
- [ ] CHK008 Are requirements stated for behavior when undoing an `AddToFight` whose initiative was inherited (no prompt was shown originally)? [Coverage, Spec §US3 AC4]

## Monster Counter Decrement on Remove

- [ ] CHK009 Is the requirement that `_monsterCountByOriginalId` decrements on `Remove` and that the key is deleted at zero explicitly stated? [Completeness, Spec §FR-011a, data-model §FightContext]
- [ ] CHK010 Is the symmetric requirement that `Restore` re-increments the counter (and only for monsters) defined? [Completeness, data-model §FightContext.Restore]
- [ ] CHK011 Is the post-condition specified that after removing the *last* monster of a kind, the next add of that template re-prompts for initiative? [Clarity, Spec §FR-011a, Edge Cases]
- [ ] CHK012 Is the post-condition specified that with count > 0, subsequent adds inherit initiative from a remaining same-kind monster (not from the removed one)? [Clarity, Spec §FR-011a]
- [ ] CHK013 Is the undo behavior of `AddToFightCommand` for a non-first-of-kind monster explicit about the counter (decrement) and explicit that the original same-kind monster must remain untouched? [Completeness, Spec §US3 AC4]
- [ ] CHK014 Is the heuristic for "is monster" inside `Restore` (used to gate counter increment) defined unambiguously? [Ambiguity, data-model §FightContext.Restore]

## Initiative Prompt Query Flow

- [ ] CHK015 Is the same-kind detection rule defined as "shares originating template id" with no other tie-breakers? [Clarity, Spec §FR-007, Assumptions]
- [ ] CHK016 Is the order of checks in `AddToFightCommandHandler` specified (same-kind check before prompt, prompt skipped only on hit)? [Clarity, data-model §AddToFightCommandHandler]
- [ ] CHK017 Are the cancel and confirm return shapes of `InitiativeRollQuery` defined (`Canceled(0)` vs `Success(int)`)? [Completeness, data-model §InitiativeRollQuery, §InitiativeRollQueryHandler]
- [ ] CHK018 Is it specified that on cancel the `AddToFight` command is neither persisted in history nor mutates state? [Consistency, Spec §FR-008, data-model §AddToFightCommandHandler]
- [ ] CHK019 Is the data the modal must present (e.g., dexterity modifier source for player vs monster template) specified? [Completeness, data-model §InitiativeRollQueryHandlerModal]
- [ ] CHK020 Is the query's return type (raw d20 int) consistent with how `AddToFightCommandHandler` consumes it (assigned to `fighter.InitiativeRoll`)? [Consistency, data-model]
- [ ] CHK021 Is the boundary between query handler (UI) and command handler (Business) clear about who owns the dialog lifecycle? [Clarity, data-model §UI Layer]

## FightingCharacter Exposing Originating Template Id

- [ ] CHK022 Is the new `OriginalCharacterId` property's type, accessibility, and immutability defined? [Completeness, data-model §FightingCharacter]
- [ ] CHK023 Is the rule for what value to assign defined for both players (`character.Id`) and monsters (template `character.Id`, NOT clone id)? [Clarity, data-model §FightingCharacter Migration]
- [ ] CHK024 Is the requirement to propagate `OriginalCharacterId` through `FightingCharacter.Copy(IMapper)` specified? [Completeness, data-model §FightingCharacter Migration]
- [ ] CHK025 Are migration impacts on existing call sites (tests, factories that construct `FightingCharacter` directly) called out? [Coverage, data-model §FightingCharacter Migration]
- [ ] CHK026 Is the rationale for exposing the id publicly (so `AddToFightCommand` need not depend on `FightContext` private state) traceable to a functional requirement? [Traceability, Spec §FR-014]
- [ ] CHK027 Is the relationship between the new public `OriginalCharacterId` and the existing private `_monsterCountByOriginalId` keying explicitly stated as the same value domain? [Consistency, Assumptions]

## Page Rename: Fight → FightDashboard

- [ ] CHK028 Are all rename targets enumerated (route, file names, class name, navigation label, user-visible references)? [Completeness, Spec §FR-002, data-model §FightDashboardPage]
- [ ] CHK029 Is the new route value specified (`/fight-dashboard`) and disambiguated from the new `/fighters` route? [Clarity, data-model §FightDashboardPage, §FightersPage]
- [ ] CHK030 Is the policy on existing `/fight` deep links (no redirect required) explicitly stated? [Coverage, Assumptions]
- [ ] CHK031 Is the requirement that no nav entry simply called "Fight" remains explicit? [Clarity, Spec §US5 AC1]
- [ ] CHK032 Are the FightDashboard behaviors that must remain unchanged after rename specified (single exception: graceful null selection)? [Consistency, Spec §FR-016]

## Removal of Legacy AddToFight Button

- [ ] CHK033 Is the exact location of the buttons to remove specified (Players panel and Monsters panel of `CharacterListEditorPage`)? [Clarity, data-model §CharacterListEditorPage]
- [ ] CHK034 Is the cleanup rule for now-unused injections (e.g., `IFightContext` on `CharacterListEditorPage`) defined with a verification step? [Completeness, data-model §CharacterListEditorPage]
- [ ] CHK035 Is "single entry point for adding to the fight" defined as a measurable success criterion? [Measurability, Spec §SC-004]
- [ ] CHK036 Are there any remaining non-Fighters-page paths that could add to the fight that the spec must explicitly forbid? [Coverage, Spec §FR-013]

## Left/Right Panel Filtering & Sorting

- [ ] CHK037 Is the left-panel filter rule for players ("hide if already in fight") specified at the source (filtered out, not just disabled)? [Clarity, Spec §FR-003]
- [ ] CHK038 Is the left-panel rule that monsters are ALWAYS shown (independent of how many of that template are in the fight) explicit? [Clarity, Spec §FR-003, Edge Cases]
- [ ] CHK039 Is the right-panel sort key, direction, and tie-break behavior specified (`InitiativeRoll` desc, ties = stable insertion order)? [Clarity, Spec §FR-004]
- [ ] CHK040 Is the requirement that sort/filter applies *within* visual groups (Players group / Monsters group), not across groups, stated? [Clarity, Spec §FR-003, §FR-004]
- [ ] CHK041 Is the reactive update requirement defined for both panels on add and on remove (no manual refresh)? [Completeness, Spec §FR-015]
- [ ] CHK042 Are the events the page must subscribe to for reactivity enumerated (`OnFighterAdded`, `OnFighterRemoved`)? [Completeness, data-model §FightersPage Lifecycle]
- [ ] CHK043 Is the requirement that subscriptions are torn down on `Dispose` to prevent leaks specified? [Completeness, data-model §FightersPage Lifecycle]
- [ ] CHK044 Is the empty-state behavior of the left panel defined (empty character repository)? [Edge Case, Spec §Edge Cases]
- [ ] CHK045 Are visual grouping requirements for Players vs Monsters defined for BOTH panels with measurable criteria (section header? divider? chip?) or explicitly deferred to implementation? [Ambiguity, Spec §FR-003, §FR-004, Assumptions]

## Dialog Service Exposure on Fighters Page

- [ ] CHK046 Is the requirement to set `IDialogServiceProvider` with the page's own `IDialogService` stated, mirroring `FightPage`? [Completeness, Spec §FR-012, data-model §FightersPage]
- [ ] CHK047 Is the lifecycle hook that performs the registration specified (`OnInitializedAsync`) and the teardown rule (if any) addressed? [Clarity, data-model §FightersPage Lifecycle]
- [ ] CHK048 Is the cross-page contract clear about what happens to the global provider when navigating away from Fighters and onto FightDashboard (which also sets it)? [Coverage, Gap]
- [ ] CHK049 Is it specified that `InitiativeRollQueryHandler` retrieves the dialog service through `IDialogServiceProvider.GetDialogService()` (and not via direct injection), so the same handler works regardless of which page initiated? [Consistency, data-model §InitiativeRollQueryHandler]

## Logging via DnD Logging System

- [ ] CHK050 Is the log entry produced by `AddToFightCommand` specified (text content, BBCode tags, that it is dispatched via `WriteLogCommand` as a sub-command)? [Completeness, data-model §AddToFightCommandHandler]
- [ ] CHK051 Is the log entry produced by `RemoveFromFightCommand` specified equivalently? [Completeness, data-model §RemoveFromFightCommandHandler]
- [ ] CHK052 Is the requirement that log entries are nested under the parent command via `SendAsSubCommandAsync(parentCommand: command)` explicit, so undo/redo cascades correctly? [Clarity, data-model]
- [ ] CHK053 Is the undo behavior on log entries (cascade hides them) consistent with the existing log block/scope semantics? [Consistency, data-model §State Transitions]
- [ ] CHK054 Is it stated that no log entry is written when the initiative prompt is cancelled (since no mutation occurred)? [Coverage, Spec §FR-008]
- [ ] CHK055 Are the semantic color tokens / formatting tags used in the new log messages aligned with the project's logging conventions, or explicitly deferred? [Ambiguity, Gap]

## Cross-Cutting / Traceability

- [ ] CHK056 Does every functional requirement (FR-001..FR-017) have at least one corresponding data-model entry or page section? [Traceability]
- [ ] CHK057 Are non-goals explicitly stated and consistent across spec and data-model (no auto-navigation, no turn auto-advance, no confirmation on remove, no bulk add, no keyboard shortcuts)? [Consistency, Spec §FR-011b, §FR-017, Clarifications, Assumptions]
- [ ] CHK058 Are success criteria SC-001..SC-005 measurable without requiring implementation inspection? [Measurability, Spec §Success Criteria]

## Notes

- Check items off as the spec is reviewed: `[x]`
- Use `[Gap]` items to drive spec amendments before implementation begins.
- This checklist tests the requirements, not the code.
