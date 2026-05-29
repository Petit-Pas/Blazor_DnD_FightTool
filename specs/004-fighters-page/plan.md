# Implementation Plan: Fighters Page

**Branch**: `004-fighters-page` | **Date**: 2026-04-28 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/004-fighters-page/spec.md`

## Summary

Introduce a new **Fighters** page (route `/fighters`) dedicated to composing the fight roster: a left panel lists addable characters from `ICharacterRepository` (players already in the fight are filtered out; monster templates are always shown), a right panel lists the current `FightContext.Fighters` sorted by `InitiativeRoll` descending. `+`/`-` buttons dispatch two new undoable commands — `AddToFightCommand` and `RemoveFromFightCommand` — through `IUndoableMediator`. The add-flow uses a new query (`InitiativeRollQuery`) backed by a new modal in `DnDQueryPrompter` to capture the initiative; the prompt is skipped when a same-template monster is already in the fight (initiative is inherited). The legacy `Fight` page is renamed to **FightDashboard** (route `/fight-dashboard`, navigation label updated). The `AddToFight` button on the character list editor is removed. `FightingCharacter` gains a public `OriginalCharacterId` so the same-kind detection no longer relies on `FightContext` private state. `IFightContext` gains an `OnFighterAdded` event to drive reactive re-rendering of both panels. All log entries use the existing DnD log system through `WriteLogCommand` sub-commands.

## Technical Context

**Language/Version**: C# 14 / .NET 10 (`net10.0`)  
**Primary Dependencies**: UndoableMediator `2.0.0-alpha3`, MudBlazor v8.x, Mapster (via `IMapper`)  
**Storage**: In-memory only — `IFightContext` (singleton) is the source of truth; no persistence change.  
**Testing**: NUnit 4 + FluentAssertions 7 + FakeItEasy 9; bUnit not introduced by this feature (out of scope unless component tests are explicitly requested).  
**Target Platform**: .NET MAUI Hybrid (Windows-first), Blazor.  
**Project Type**: Desktop app (MAUI Hybrid) with shared Blazor UI.  
**Performance Goals**: N/A — single-user desktop, lists are small (<100 entries typical).  
**Constraints**: No new NuGet dependencies. No new project. Reuse existing `IDialogServiceProvider`/`IDialogService` pattern from `FightPage`. Reuse existing undo/redo infrastructure (feature 003).  
**Scale/Scope**: One new page, two new commands (+ handlers, + tests), one new query (+ handler + modal), one new event on `IFightContext`, one new public property on `FightingCharacter`, rename of one page, deletion of one button.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate | Status | Notes |
|---|---|---|
| No new dependencies | PASS | Only existing packages reused. |
| File-scoped namespaces | PASS | All new files use file-scoped namespaces. |
| UndoableMediator for all mutations | PASS | `AddToFightCommand` and `RemoveFromFightCommand` go through `IUndoableMediator`. The character-list-editor's direct `FightContext.Add` call is being **removed**, eliminating the only remaining non-mediator mutation path into the fight. |
| Domain has no external deps | PASS | The new event on `IFightContext` and the new property on `FightingCharacter` stay within `Domain/Fight`. |
| Layer dependency direction | PASS | Page → Commands (Business) → Domain. Query handler in `DnDQueryPrompter` (UI) depends on Business + Domain only. |
| No logic in `.razor` files | PASS | All C# in `FightersPage.razor.cs`, `InitiativeRollQueryHandlerModal.razor.cs`, etc. |
| New commands placed under `src/Business/DnDActions/{DnDEntity}Actions/` | PASS | Both commands live under `src/Business/DnDActions/FightActions/{AddToFight,RemoveFromFight}/`. (`Fight` is the `Domain/Fight` analogue; mirrors the existing `TurnActions/` precedent.) |
| New query definitions in `src/Business/DnDQueries/{DnDEntity}/` | PASS | `InitiativeRollQuery` lives in `src/Business/DnDQueries/FightQueries/`. |
| Query handlers in `src/UI/DnDQueryPrompter/{DnDEntity}Queries/` | PASS | `InitiativeRollQueryHandler` and modal live in `src/UI/DnDQueryPrompter/FightQueries/`. |
| Page sets `IDialogServiceProvider` | PASS | `FightersPage.OnInitializedAsync` calls `DialogServiceProvider.SetDialogService(DialogService)` (mirrors `FightPage`). |
| DI via `ServiceCollectionExtensions` | PASS | UndoableMediator scans assemblies; new handlers are picked up automatically. No new `Register*` extension introduced; existing registrations remain sufficient. |
| Tests for command handlers | PASS | `AddToFightCommandHandlerTests`, `RemoveFromFightCommandHandlerTests` planned with `Execute`/`Undo`/`Redo` nested classes per existing convention. |
| Blazor components inherit `StylableComponentBase` if exposing Class/Style | PASS | The page does not expose `Class`/`Style`; helper components (e.g., a fighter-row item) follow the same rule when applicable. |
| `IFightContext` is the source of truth | PASS | The new event `OnFighterAdded` is added to `IFightContext`; the per-template counter remains internal to `FightContext` but is driven by command handlers via existing `Add`/`Remove` methods (no new public mutation API). |
| Monsters cloned on add, players added by reference | PASS | `FightContext.Add` already enforces this; commands route through it unchanged. |
| Fight-specific extensions in `src/Domain/Fight/DomainExtensions/` | PASS | If any same-kind lookup helper is extracted, it goes there. Otherwise the lookup is a one-liner inside the command handler. |

**No violations. Gate passed.**

**Post-design constitution re-check** (after Phase 1): No additional violations. The single area of design tension — exposing the originating template id on `FightingCharacter` — is explicitly mandated by FR-014 and is the lesser evil compared to leaking `_monsterCountByOriginalId` out of `FightContext`. See D-002.

## Project Structure

### Documentation (this feature)

```text
specs/004-fighters-page/
├── plan.md              # This file (/speckit.plan output)
├── spec.md              # Feature specification
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks — NOT created here)
```

> No `contracts/` directory — this feature exposes no public API, no HTTP endpoint, no CLI surface. Its interfaces are internal C# (`IFightContext`, `FightingCharacter`, command/query types) which are documented in `data-model.md`.

### Source Code Changes

```text
src/
├── Domain/
│   └── Fight/
│       ├── FightContext.cs                                       # MODIFIED — add OnFighterAdded event; raise it from Add(...)
│       ├── IFightContext.cs                                      # MODIFIED — declare OnFighterAdded
│       └── Characters/
│           └── FightingCharacter.cs                              # MODIFIED — add public Guid OriginalCharacterId { get; }
│
├── Business/
│   ├── DnDActions/
│   │   └── FightActions/                                         # NEW folder
│   │       ├── AddToFight/
│   │       │   ├── AddToFightCommand.cs                          # NEW
│   │       │   └── AddToFightCommandHandler.cs                   # NEW
│   │       └── RemoveFromFight/
│   │           ├── RemoveFromFightCommand.cs                     # NEW
│   │           └── RemoveFromFightCommandHandler.cs              # NEW
│   └── DnDQueries/
│       └── FightQueries/                                         # NEW folder
│           └── InitiativeRollQuery.cs                            # NEW
│
├── UI/
│   └── DnDQueryPrompter/
│       └── FightQueries/                                         # NEW folder (mirrors DnDQueries/FightQueries)
│           ├── InitiativeRollQueryHandler.cs                     # NEW
│           ├── InitiativeRollQueryHandlerModal.razor             # NEW
│           ├── InitiativeRollQueryHandlerModal.razor.cs          # NEW
│           └── InitiativeRollQueryHandlerModal.razor.css         # NEW (optional, may be empty)
│
└── Components/
    └── DndUi.Shared/
        └── Components/
            ├── Layout/
            │   └── NavMenu.razor                                 # MODIFIED — add "Fighters" link, rename "Fight" link to "FightDashboard", update HRefs
            └── Pages/
                ├── CharacterListEditorPage.razor                 # MODIFIED — remove FightButton + AddToFight click handler
                ├── CharacterListEditorPage.razor.cs              # MODIFIED — remove AddToFight method + IFightContext injection (if no longer used)
                ├── FightPage.razor                               # RENAMED → FightDashboardPage.razor; route changed from "/fight" to "/fight-dashboard"
                ├── FightPage.razor.cs                            # RENAMED → FightDashboardPage.razor.cs; class renamed to FightDashboardPage; null-selection handling for SelectedFighter
                ├── FightersPage.razor                            # NEW — left/right two-column layout with grouped lists
                ├── FightersPage.razor.cs                         # NEW — DI for IUndoableMediator, IFightContext, ICharacterRepository, IDialogService(Provider); event subscriptions
                └── FightersPage.razor.css                        # NEW — two-column layout + group-header styling

tests/
├── Business/
│   └── DnDActionsTests/
│       └── FightActions/                                         # NEW folder
│           ├── AddToFightCommandHandlerTests.cs                  # NEW — Execute/Undo/Redo nested classes
│           └── RemoveFromFightCommandHandlerTests.cs             # NEW — Execute/Undo/Redo nested classes
└── Domain/
    └── FightTests/
        └── FightContextTests.cs                                  # MODIFIED — add OnFighterAdded coverage; OriginalCharacterId assertions
```

**Structure decision**: No new projects. New code lives in existing projects under new feature-named folders that mirror existing conventions (`TurnActions/`, `SaveQueries/`). The `FightPage` rename is a file rename + class rename + route change + nav-link update; no behavioral change beyond null-selection rendering (FR-011b/FR-016). Query handlers stay in `DnDQueryPrompter` per Constitution §9.

## Complexity Tracking

No constitution violations — this table is intentionally empty.

## Key Design Decisions

### D-001: Two top-level commands, no orchestrator

`AddToFightCommand` and `RemoveFromFightCommand` are both top-level commands sent via `IUndoableMediator.SendAsync`. They are not sub-commands of anything. This matches user intent: each click is one undoable user action.

The `WriteLogCommand` calls inside each handler are sub-commands (per the dnd-logging skill).

### D-002: Expose `OriginalCharacterId` on `FightingCharacter` (FR-014)

The same-kind detection in `AddToFightCommandHandler` requires looking up "is there already a monster with the same originating template id in the fight?". Two options were considered:

1. **Expose `OriginalCharacterId` publicly on `FightingCharacter`** (chosen). The id is already implicitly known: for players, it's `_character.Id`; for monsters, the cloned character's id is regenerated, so `FightContext.Add` must remember the **source** id and pass it to the `FightingCharacter` constructor. Add a constructor parameter `Guid originalCharacterId` and store it.
2. Expose a method on `IFightContext` like `bool HasMonsterFromTemplate(Guid templateId)` that consults `_monsterCountByOriginalId`. Rejected: spec FR-014 explicitly requires the data to live on `FightingCharacter`, and option 1 makes the data observable for any consumer (UI grouping, future features) without leaking private state.

Implication: `FightContext.Add(Character)` is updated to pass `character.Id` as `originalCharacterId` to the `FightingCharacter` constructor. This is a non-breaking internal change — no public API change of `IFightContext.Add`.

### D-003: Initiative prompt is a query, not a dialog raised by the page

`InitiativeRollQuery : QueryBase<int>` is dispatched from inside `AddToFightCommandHandler` via `_mediator.QueryAsync(...)`. The query handler (`InitiativeRollQueryHandler` in `DnDQueryPrompter`) opens the modal through `IDialogServiceProvider`. The page's only responsibility w.r.t. dialogs is to set the dialog service:

```csharp
DialogServiceProvider.SetDialogService(DialogService);
```

This satisfies FR-007, FR-008, FR-012 and matches the `SaveRollResultQuery` pattern.

**Cancellation handling (FR-008)**: If the modal is cancelled, the query handler returns `QueryResponse<int>.Canceled(0)`. `AddToFightCommandHandler.ExecuteAsync` checks `Status == RequestStatus.Canceled` **before any state mutation**, returns `CommandResponse.Canceled()`, and no entry is added to undo history (per UndoableMediator rules — `Canceled` is not recorded).

### D-004: Same-kind detection precedes the prompt

`AddToFightCommandHandler.ExecuteAsync` algorithm:

1. Look up the source `Character` by id from `ICharacterRepository`.
2. If `character.Type == Monster` AND `_fightContext.Fighters.Any(f => f.OriginalCharacterId == character.Id)`:
   - Inherit initiative from the first matching fighter (`InitiativeRoll`).
   - Skip the prompt.
3. Else:
   - Send `InitiativeRollQuery` via `_mediator.QueryAsync`. On cancel → return `Canceled` (no state mutation).
4. Call `_fightContext.Add(character)` (this also bumps the monster counter and clones for monsters).
5. Find the just-added fighter (last one whose `OriginalCharacterId == character.Id` and not yet in the prior set), set `InitiativeRoll`, store `AddedFighterId` on the command for undo.
6. Emit log entries via `WriteLogCommand` sub-commands.

Steps 4–5 must run in the same handler so the inheritance check uses the **state at execute time** (not at command construction time). Storing `OriginalCharacterId` on the fighter (D-002) makes step 5 unambiguous.

### D-005: Undo of `AddToFightCommand` reuses `FightContext.Remove`

`UndoAsync` calls `_fightContext.Remove(addedFighter)`. This automatically decrements per-template counters because `FightContext.Remove` is updated to mirror the increment in `Add` (see D-006). No bespoke counter management in the command handler.

### D-006: `FightContext.Remove` decrements the per-template counter (FR-011a)

`Remove(FightingCharacter)` is updated to:
1. Look up `OriginalCharacterId` on the fighter being removed.
2. If the source character is a monster (or simply: if the counter dictionary contains the key), decrement; remove the key when it reaches 0.
3. Continue with current behavior (remove from `_fighters`, fire `OnFighterRemoved`).

This is the only behavior change to `FightContext` beyond the new event and the constructor wiring of `OriginalCharacterId`.

### D-007: Re-add the exact same `FightingCharacter` instance on undo of `RemoveFromFightCommand` (FR-011)

To restore HP, statuses, name suffix ("Goblin 3"), the **same instance** is re-inserted. `RemoveFromFightCommand` stores a reference to the `FightingCharacter` (not just its id) on `ExecuteAsync`. `UndoAsync` calls a new internal method on `IFightContext` — `Restore(FightingCharacter)` — that:
1. Inserts back into `_fighters[fighter.Id]`.
2. Re-increments `_monsterCountByOriginalId[fighter.OriginalCharacterId]` if the source is a monster.
3. Fires `OnFighterAdded`.

Rationale for a separate `Restore` rather than reusing `Add(Character)`: `Add` clones monsters and renames them, which would create a *new* fighter with a different id and reset state. Restore is a faithful reinsertion.

### D-008: Sort the right-hand list at presentation time (FR-004)

`FightContext.Fighters` enumeration order remains insertion-based. Sorting by `InitiativeRoll` descending (with stable insertion-order tie-break) is done in the page's render path:

```csharp
FightContext.Fighters.OrderByDescending(f => f.InitiativeRoll)
```

`OrderByDescending` is stable in LINQ-to-Objects, so insertion order is preserved on ties.

### D-009: Active-fighter null-out on removal (FR-011b)

`RemoveFromFightCommandHandler.ExecuteAsync` checks `_combatTurnService.CurrentTurnFighter?.Id == command.FighterId`. If true, it sends a `SetCurrentFighterCommand(null)` as a sub-command. This requires changing `SetCurrentFighterCommand.FighterId` from `Guid` to `Guid?` (the underlying `ICombatTurnService.SetCurrentTurnFighter(Guid?)` already accepts null).

Sub-command propagation guarantees that undoing the remove also restores the previously-current fighter (the `SetCurrentFighter` sub-command's own undo restores the previous id) — see FR-011b.

The alternative — directly calling `_combatTurnService.SetCurrentTurnFighter(null)` in the handler — is rejected: it bypasses the mediator and breaks undo of "current fighter" alongside undo of remove.

### D-010: Players-already-in-fight filtering (FR-003)

The left list is computed reactively from `ICharacterRepository.GetAllCharacters()` joined against `FightContext.Fighters`:

- Players: `where character.Type == Player and not exists fighter with OriginalCharacterId == character.Id`
- Monsters: always shown (no filtering)

Re-render is triggered by `IFightContext.OnFighterAdded` and `OnFighterRemoved` (page subscribes to both, calls `InvokeAsync(StateHasChanged)`).

### D-011: Visual grouping (FR-004, US4)

Both the left and right panels use plain section headers (`MudText Typo="Typo.subtitle2"` plus a `MudDivider`) above each group within each panel. No collapsible expansion panels. Static headers minimize complexity and match the existing `CharacterListEditorPage` flat aesthetic.

### D-012: Logging policy

Each command emits a one-line log entry via `WriteLogCommand` (no block — these are simple roster changes, not multi-step actions):

- Add: `"[b]{fighter.Name}[/b] joined the fight (initiative [b]{initiative}[/b])"`
- Remove: `"[b]{fighter.Name}[/b] left the fight"`

No color tokens (these are not damage/heal events).

### D-013: FightDashboard rename scope

The rename touches:
- File rename: `FightPage.razor`/`.razor.cs` → `FightDashboardPage.razor`/`.razor.cs`.
- Class rename: `FightPage` → `FightDashboardPage`.
- Route change: `@page "/fight"` → `@page "/fight-dashboard"`.
- `NavMenu.razor`: replace `Fight` link's `HRef="fight"` with `HRef="fight-dashboard"` and label `"FightDashboard"`; insert a new `Fighters` link with `HRef="fighters"`.
- Existing `CheckForFightersWithoutInitiative` logic remains as a safety net (no-op when all fighters have initiative, which is now guaranteed by `AddToFightCommand`).
- Null-`_selectedFighter` rendering: the existing render path already handles a null `_selectedFighter` (cascade is `null`, `FightingCharacterTile` selection check guards on `IsStarted`). Confirmed safe; no `_selectedFighter` access without null-check is introduced.
