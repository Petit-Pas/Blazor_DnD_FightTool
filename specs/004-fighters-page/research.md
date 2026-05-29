# Research: Fighters Page

All items below resolve **NEEDS CLARIFICATION** entries from the Technical Context, plus open architectural questions surfaced by the spec.

## R-001: Where to store the originating character template id

**Decision**: Add a public read-only `Guid OriginalCharacterId { get; }` to `FightingCharacter`, set via the constructor. `FightContext.Add` passes `character.Id` (the source id) to the new constructor parameter, **before** the monster clone gets a new id. For players, it equals `_character.Id`. For monsters, it equals the *source template's* `Character.Id`, not the cloned copy's id.

**Rationale**: FR-014 explicitly requires this exposure. It removes the need for command handlers to consult `FightContext._monsterCountByOriginalId` (private) and provides a clean foundation for left-list filtering (D-010) and same-kind detection (D-004).

**Alternatives considered**:
- Expose a `IFightContext.HasFighterFromTemplate(Guid)` query method — rejected (FR-014 explicitly mandates the data live on the fighter).
- Store on the underlying `Character` (not `FightingCharacter`) — rejected; the cloned monster's `Character.Id` is regenerated on clone, losing the linkage.

**Resolution**: Implementation step 1.

---

## R-002: Counter management on remove and on undo-of-add

**Decision**: Update `FightContext.Remove(FightingCharacter)` to decrement `_monsterCountByOriginalId[fighter.OriginalCharacterId]`, removing the key when count reaches 0. Both `AddToFight.Undo` and `RemoveFromFight.Execute` then route through the existing `Remove` method, getting correct counter behavior for free. Restoration on undo of `RemoveFromFight` re-increments via a new `Restore(FightingCharacter)` method on `IFightContext`.

**Rationale**: FR-011a, FR-009. Putting the counter logic inside `FightContext` keeps the per-template invariant in one place. Command handlers stay free of internal bookkeeping.

**Alternatives considered**:
- Each command handler manages the counter via reflection / a separate map — rejected (leaks internal state, fragile).
- Replace the counter with a derived `Fighters.Count(f => f.OriginalCharacterId == id)` lookup — viable and arguably cleaner (the counter would become *derived*), but it changes the monster naming logic (`$"{character.Name} {count}"`) which would then need to be `Count(...) + 1`. This is a larger refactor; deferred. Counter logic stays for now.

**Resolution**: Update `FightContext.Remove`; add `IFightContext.Restore` (used only by `RemoveFromFight` undo).

---

## R-003: How to obtain initiative on Add

**Decision**: New query `InitiativeRollQuery : QueryBase<int>` with handler `InitiativeRollQueryHandler` in `DnDQueryPrompter/FightQueries/`. The handler opens `InitiativeRollQueryHandlerModal` via `IDialogServiceProvider.GetDialogService()`. The modal contains a single d20 input plus the dexterity modifier of the character being added (purely informational; the returned value is the raw d20). Returns `int` (the raw `InitiativeRoll`); cancel returns `QueryResponse<int>.Canceled(0)`.

**Rationale**: FR-007, FR-012. Mirrors the `SaveRollResultQuery` pattern verbatim. Keeps dialog logic out of the command handler and out of the page.

**Alternatives considered**:
- Reuse the existing `InitiativeInputDialog` (multi-fighter table) — rejected: that dialog is bulk-fill-after-the-fact for fighters with `InitiativeRoll == 0`. The new flow is per-add, single-fighter, so a dedicated single-row modal is simpler and matches the spec UX.
- Pass the initiative as a constructor parameter to `AddToFightCommand` (page prompts before dispatching) — rejected: violates FR-007 ("via a query through the page's DialogService") and would force the page to know about the same-kind inheritance rule.

**Resolution**: New query + handler + modal.

---

## R-004: Restoring exact state on undo of remove (FR-011)

**Decision**: `RemoveFromFightCommand` stores a reference to the `FightingCharacter` instance removed (not a clone, not just an id). `UndoAsync` calls `IFightContext.Restore(fighter)`, which re-inserts the same instance, restores the counter, and fires `OnFighterAdded`.

**Rationale**: `FightingCharacter` holds mutable state (HP, statuses) that the user may have changed before removal. Persisting the *reference* is the simplest faithful restore. The instance is held alive by the undo-history command list (UndoableMediator references it).

**Alternatives considered**:
- Deep-clone the fighter in `Execute` and re-insert the clone in `Undo` — rejected: clone semantics for `FightingCharacter` are non-trivial (it wraps a `Character`); see existing `FightingCharacter.Copy(IMapper)`. Holding the original is sufficient.
- Snapshot the fighter as a DTO — rejected: no DTO exists, and creating one is unjustified for an in-memory single-user app.

**Resolution**: Add `void Restore(FightingCharacter)` to `IFightContext`/`FightContext`. Used only by `RemoveFromFightCommand.UndoAsync`.

---

## R-005: Active-fighter null-out across undo

**Decision**: `RemoveFromFightCommandHandler` sends a `SetCurrentFighterCommand(null)` as a **sub-command** when the removed fighter is the current turn fighter. `SetCurrentFighterCommand.FighterId` is changed from `Guid` to `Guid?` to permit this; the existing handler already accepts `Guid?` via `ICombatTurnService.SetCurrentTurnFighter(Guid?)`.

**Rationale**: FR-011b. Sub-command propagation guarantees that undoing the remove also restores the previously-current fighter (the `SetCurrentFighter` sub-command's own undo restores the previous id).

**Alternatives considered**:
- Direct `_combatTurnService.SetCurrentTurnFighter(null)` call — rejected: bypasses the mediator and breaks undo of "current fighter" alongside undo of remove.
- Add a separate `ClearCurrentFighterCommand` — rejected: redundant with making `FighterId` nullable.

**Resolution**: Make `SetCurrentFighterCommand.FighterId` nullable. Update `SetCurrentFighterCommandHandlerTests` accordingly. This is a small, low-risk change — it expands the input domain without breaking existing call sites (all current callers pass non-null `Guid` values, which still compile against `Guid?`).

---

## R-006: Reactive UI updates without manual refresh (FR-015)

**Decision**: Add `event EventHandler<FightingCharacter>? OnFighterAdded` to `IFightContext`. `FightContext.Add` and `FightContext.Restore` raise it. `FightersPage` subscribes to both `OnFighterAdded` and `OnFighterRemoved` and calls `InvokeAsync(StateHasChanged)` on each.

**Rationale**: Mirrors the existing `OnFighterRemoved` pattern. No new infrastructure (no Rx, no polling).

**Alternatives considered**:
- Single `OnFightersChanged` event with no payload — viable; rejected only because the existing pair (`OnFighterRemoved`, `OnFighterUpdated`) already follows the granular per-action style. Consistency wins.

**Resolution**: One new event, two raise sites.

---

## R-007: Visual grouping primitive

**Decision**: Use plain section headers (`MudText Typo="Typo.subtitle2"` + `MudDivider`) above each group within each panel. No `MudExpansionPanel`/collapsible behavior.

**Rationale**: The spec calls for *visual* grouping, not interactive collapsing. Static headers minimize complexity and match the existing `CharacterListEditorPage` tabbed-but-flat aesthetic.

**Alternatives considered**:
- `MudExpansionPanel` per group — rejected as unrequested feature creep.
- Single grid with a "Type" column header sort — rejected; the spec explicitly says grouped.

**Resolution**: Plain headers + dividers. Exact styling deferred to implementation.

---

## R-008: Existing `CheckForFightersWithoutInitiative` on FightDashboard

**Decision**: Keep it untouched. After this feature, every fighter is added with a non-zero `InitiativeRoll` (or the add was cancelled). The check becomes effectively a no-op safeguard. FR-016 requires unchanged behavior aside from rename and null-selection rendering.

**Rationale**: Removing the check is out of scope and would add a coordination risk (e.g., if any other code path were to add a fighter without an initiative, the safeguard catches it).

**Resolution**: No code change inside FightDashboard's render flow beyond the rename.

---

## R-009: Route conflict — current `@page "/"` on `CharacterListEditorPage`

**Observation**: `CharacterListEditorPage` is registered at both `"/"` and `"/Characters"`. Adding a `/fighters` route does not conflict.

**Decision**: Use route `"/fighters"` for `FightersPage` and `"/fight-dashboard"` for the renamed page. The home route stays on the character list editor.

**Rationale**: Spec FR-001/FR-002 require named navigation entries but do not mandate the home page change.

**Alternatives considered**:
- Make `/fighters` the home route — rejected; out of scope.

**Resolution**: Two new explicit routes; home route untouched.

---

## R-010: Test factories for `FightingCharacter` with `OriginalCharacterId`

**Decision**: Update `tests/Domain/DomainTestsUtilities` factories to populate `OriginalCharacterId` (default to a fresh `Guid.NewGuid()` matching the wrapped `Character.Id` for player-style fighters; explicit override available for monster-style same-kind tests).

**Rationale**: New tests for `AddToFightCommandHandler` need a way to pre-seed a `FightContext` with a same-kind monster. Existing factories must reflect the new constructor.

**Resolution**: Adjust the relevant factory in `DomainTestsUtilities` as part of implementation.

---

## R-011: Logging style for add/remove

**Decision**: Single-line `WriteLogCommand` per action, no block. Content:
- Add: `[b]{fighter.Name}[/b] joined the fight (initiative [b]{initiative}[/b])`
- Remove: `[b]{fighter.Name}[/b] left the fight`

**Rationale**: These are atomic events, not multi-step orchestrations. Per the dnd-logging skill, blocks are reserved for grouped multi-line output (attacks, multi-target effects).

**Alternatives considered**:
- Open a "Roster" block once and append entries — rejected; the block lifecycle does not align with discrete user actions over time.

**Resolution**: Send `WriteLogCommand` as a sub-command in each handler's `ExecuteAsync` (no `OpenBlock`/`CloseBlock` pair).
