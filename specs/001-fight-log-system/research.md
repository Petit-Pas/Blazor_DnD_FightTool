# Research: DnD Log System

**Feature**: 001-fight-log-system  
**Date**: 2026-04-24

## R-001: Where should `IDnDLogService` and entities live?

**Decision**: New project `src/Domain/Logs/Logs.csproj` (namespace `DnDFightTool.Domain.Logs`)

**Rationale**:
- The log is not fight-scoped (singleton, persists across fights), so it doesn't belong in `Fight`.
- It has no dependency on `CharacterSheet`, `Fight`, or `Rolls` — it's a standalone domain concept.
- The constitution requires Domain projects to have no external deps (only infrastructure). A new project satisfies this cleanly.
- `IDnDLogService` (interface), `LogEntry`, `LogBlock` are pure domain types. The concrete `DnDLogService` also lives here (simple in-memory implementation, no external deps).
- Named `Logs` (not `DnDLog`) because we're already in a DnD application — the prefix is redundant.

**Alternatives considered**:
- Put in `Fight` project → rejected: logs are not fight-scoped; creates unnecessary coupling.
- Put in `Infrastructure` → rejected: it's domain logic, not cross-cutting utility.
- No new project, just a folder in `Fight` → rejected: violates the scoping clarification ("not fight-scoped").
- Name it `DnDLog` → rejected: redundant `DnD` prefix within a DnD app.

## R-002: BBCode tag parser approach

**Decision**: Hand-rolled single-pass tokenizer in `FightBlazorComponents/Log/Parsing/`

**Rationale**:
- The tag set is tiny (3 tags: `[b]`, `[c:token]`, `[hover:text]`) and fully controlled by us.
- A regex or single-pass character scanner is sufficient — no need for a full parser combinator library.
- Placing it in the UI layer (not Domain) keeps it a presentation concern. Domain stores raw tokenized strings; UI parses and renders.
- No new dependency needed.

**Alternatives considered**:
- NuGet BBCode parser (e.g., CodeKicker.BBCode) → rejected: adds dependency for 3 tags; many BBCode libs don't support custom tags like `[hover:]`.
- Markdown subset → rejected: no standard Markdown for color or hover.
- HTML subset → rejected: XSS risk if we embed raw HTML; requires sanitization complexity.

## R-003: Undo/redo integration for log writes

**Decision**: `WriteLogCommand` is a sub-command dispatched via `SendAsSubCommandAsync`. Its `UndoAsync` calls `IDnDLogService.Hide(logEntryId)`; `RedoAsync` calls `IDnDLogService.Show(logEntryId)`.

**Rationale**:
- UndoableMediator cascades undo/redo to sub-commands automatically when the parent is undone.
- No explicit "hide" or "show" command is needed — visibility is a side effect of the standard undo/redo flow.
- The command stores only the `Guid` of the created entry (not the entry object itself). Commands must be serializable — storing domain objects in command state is not allowed.
- Visibility state is managed by the service (internal `HashSet<Guid>` of hidden IDs), not on the `LogEntry` entity. This keeps `LogEntry` a simple, immutable data record.
- `OpenBlock`, `CloseBlock`, `OpenScope`, and `CloseScope` ARE commands dispatched via `SendAsSubCommandAsync`, like `WriteLogCommand`. Block commands carry meaningful undo behaviour: `OpenBlockCommandHandler.UndoAsync` calls `_logService.CloseBlock()` (reverses the opening); `CloseBlockCommandHandler.UndoAsync` calls `_logService.ReopenBlock(command.ClosedBlockId)` (reverses the closing by restoring the closed block as current). To enable this, `IDnDLogService.CloseBlock()` returns the `Guid` of the block it closed, which `CloseBlockCommand` stores as `ClosedBlockId`.
- `OpenScopeCommandHandler.UndoAsync` and `CloseScopeCommandHandler.UndoAsync` are no-ops — indentation level is entirely positional and reconstructed from scope depth at render time; there is no independent scope state to reverse.

**Alternatives considered**:
- Make block open/close direct service calls (not commands) → rejected: then their undo cannot participate in the mediator's sub-command cascade, preventing turn-block re-opening on undo.
- Store `IsVisible` on the `LogEntry` entity → rejected: commands must be serializable and should only store GUIDs, not domain objects. Visibility is a service-level concern.

## R-004: CSS theming for semantic color tokens

**Decision**: A dedicated `log-colors.css` file defining `--log-color-{token}` CSS custom properties, using MudBlazor's dark/light mode cascade.

**Rationale**:
- MudBlazor uses `<MudThemeProvider IsDarkMode="...">` which cascades `IsDarkMode` as a `CascadingValue`. The layout already toggles this.
- Approach: use a `.dark-mode` / `.light-mode` CSS class on a parent element (toggled by the theme cascade), with CSS custom properties scoped under each class.
- Alternative: use `@media (prefers-color-scheme)` — but this doesn't align with MudBlazor's manual toggle (user can override system preference).
- The `log-colors.css` file will be loaded via `<link>` in the shared `App.razor` or `index.html`.

**Alternatives considered**:
- Inline styles in C# → rejected: violates separation of concerns; harder to theme.
- MudBlazor `MudTheme` palette extension → rejected: MudBlazor palette doesn't have arbitrary custom properties; CSS variables are more flexible.

## R-005: Log component placement on fight page

**Decision**: Replace the `<div>fight log placeholder</div>` in `FightPage.razor` with `<DnDLogComponent />`.

**Rationale**:
- The placeholder is a simple `<div>` in grid position (row 1, column 2).
- The component subscribes to `IDnDLogService.OnChanged` and re-renders on each change.
- The component should be scrollable (auto-scroll to latest entry by default).

## R-006: DnDLogService registration

**Decision**: Singleton, registered in `Logs/IoC/ServiceCollectionExtensions.cs`, chained from `MauiProgram.cs` and `DndUi.Web/Program.cs`.

**Rationale**:
- Constitution: "Singleton: IFightContext, ICharacterRepository, IMapper, IUserInteractionService, IJsonSerializer". `IDnDLogService` follows the same pattern.
- The service persists for the app session. Users can call `Clear()` to reset.

## R-007: Semantic color tokens as an enum

**Decision**: Define a `LogColorToken` enum in `src/Domain/Logs/` listing all supported color token names. The parser validates `[c:token]` against this enum; unsupported tokens cause the entire tag to be rendered as literal text.

**Rationale**:
- An enum provides compile-time control over what color tokens are accepted.
- Command handlers use `LogColorToken.Fire` (or `.ToTagName()` extension) instead of magic strings, preventing typos.
- The parser (UI layer) converts the string token name to the enum; if parsing fails, the tag is emitted as literal text (graceful degradation with visibility — the author sees their mistake).
- The CSS variable name is derived from the enum via a convention: `LogColorToken.Fire` → `--log-color-fire`.

**Enum values**: `Bludgeoning`, `Piercing`, `Slashing`, `BludgeoningSilver`, `PiercingSilver`, `SlashingSilver`, `BludgeoningMagic`, `PiercingMagic`, `SlashingMagic`, `Acid`, `Cold`, `Fire`, `Force`, `Lightning`, `Necrotic`, `Poison`, `Psychic`, `Radiant`, `Thunder`, `Heal`

**Alternatives considered**:
- Free-form strings with CSS fallback → rejected: no compile-time safety, typos silently fall back to default color (invisible bug).
- Attribute-decorated enum mapping to DamageTypeEnum → rejected: log colors are a presentation concept and not 1:1 with DamageTypeEnum (we have `Heal` which isn't a damage type).
