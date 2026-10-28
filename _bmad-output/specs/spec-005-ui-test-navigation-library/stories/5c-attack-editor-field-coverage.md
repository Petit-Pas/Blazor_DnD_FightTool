---
title: 'Attack editor field coverage'
type: 'feature'
created: '2026-10-23'
status: 'done'
baseline_commit: '31ce4223ebf0184b46b9dc6c1b9365ee61fedf38'
review_loop_iteration: 0
context:
  - '{project-root}/_bmad-output/project-context.md'
  - '{project-root}/_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md'
---

<frozen-after-approval reason="human-owned intent — do not modify unless human renegotiates">

## Intent

**Problem:** Story 5 made the attack editor reachable, but only at name level. A test cannot drive or read the to-hit modifiers or the damage rolls (add, delete, damage type, dice), so CAP-2 stops at the attack editor's first field. This is the remainder of DW-015.

**Approach:** Extend `TestableAttackEditorPage` and `TestableAttackMainInfoEditor`, and add one `Testable{Component}` per damage component. Give the attack editor's panels and icon-only controls accessible names. Write capability-grouped `IsolatedScenarioFixture` tests in two directions: edit through the UI, Save the attack, Save the parent character, then assert the real repository; separately seed values and open the editor to assert their rendering. Add one invalid-input case per dice field. No fakes.

## Boundaries & Constraints

**Always:** Follow SPEC.md constraints and [uitests-layout.md](../uitests-layout.md): placement, naming, namespaces, `[SetUp]` fields, user-like navigation (`CharacterList.GoToAsync` → `EditPlayer` → `OpenAttacks` → `EditAttack`, written in the test or its `[SetUp]`), role + accessible name locators only, FluentAssertions only, seed every value acted on or asserted. Seed with `CharacterFactory.BuildPlayer`, `MartialAttackTemplateFactory`, `DamageRollTemplateFactory`, `ModifiersTemplateFactory`. Test bodies pass rendered strings as literals (`"Fire"`, `"2d8+STR"`). Save the attack, then the parent character, before asserting stored attacks. Repo C# rules apply.

**Ask First:** Any production change other than the accessible names in Tasks. Any `data-testid`. Any framework change. Any change to `RegexField` behaviour.

**Never:** No domain or binding change. No local base fixture, static helper or flow layer. No root-provider access to `IAttackEditContext`/`IGlobalEditContext`. No `SequentialScenarioFixture`. No UI test of the dice grammar or expression normalization. Do not migrate other DW-022 locators.

## I/O & Edge-Case Matrix

| Scenario | Input / State | Expected Output / Behavior | Error Handling |
|----------|--------------|---------------------------|----------------|
| TO_HIT | Seeded attack, to-hit `WIS+8` | Set to-hit `STR+2`, Save, Save parent; stored to-hit is `STR+2` | — |
| TO_HIT_INVALID | Seeded to-hit `WIS+8` | Type `abc`; field shows the error; Save is refused (editor stays); Cancel, Save parent; stored to-hit is still `WIS+8` | Save blocked |
| DAMAGE_ADD | Seeded one damage (Fire, `1d6`) | Add damage, Save, Save parent; two stored damages, the first still Fire `1d6` | — |
| DAMAGE_DELETE | Seeded Fire `1d6` + Cold `2d8` | Delete damage 1, Save, Save parent; only Cold `2d8` stored | — |
| DAMAGE_TYPE | Seeded Fire `1d6` | Pick `Cold`, Save, Save parent; stored type is Cold | — |
| DAMAGE_DICE | Seeded Fire `1d6` | Set dice `2d8+STR`, Save, Save parent; stored dice is `2d8+STR` | — |
| DAMAGE_DICE_INVALID | Seeded Fire `1d6` | Type `abc`; same flow as TO_HIT_INVALID; stored dice still `1d6` | Save blocked |
| DAMAGE_CANCEL | Seeded Fire `1d6` | Set dice `2d8`, Cancel attack, Save parent; stored dice still `1d6` | — |
| SEEDED_RENDERING | Seeded to-hit `WIS+8`, damages Fire `1d6` + Cold `2d8` | Open editor without editing; to-hit, damage count, each row's type and dice render | — |

Expressions shown are examples; seed and type canonical forms so the stored `Expression` equals the typed text.

</frozen-after-approval>

## Code Map

- `src/UI/CharacterSheetBlazorComponents/MartialAttacks/Pages/AttackEditorPage.razor:13-24` -- one tab: `AttackMainInfoEditor` + `DamageRollTemplateCollectionEditorComponent` + `ButtonRow` (Save/Cancel `aria-label`s). `.razor.cs:60` Save only calls `SaveEditedAttack()` when `AreAllValid()`; otherwise it stays on the page.
- `src/UI/CharacterSheetBlazorComponents/MartialAttacks/Components/AttackMainInfoEditor.razor:9-13` -- `MudPaper` with title "Main Informations"; `MudForm` with `Name` and `DiceRollModifiersTemplateEditorComponent` labelled `To Hit Modifiers`.
- `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageRollTemplateCollectionEditorComponent.razor:8-26` -- `MudPaper` titled "Damages"; unnamed add `MudIconButton`; `foreach` renders one `DamageRollTemplateEditorComponent` per template. `.razor.cs:19` `AddNew()` appends a default `DamageRollTemplate`.
- `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageRollTemplateEditorComponent.razor:7-12` -- row: `DamageTypeEditorComponent`, `DiceRollTemplateEditorComponent` (no `Label`), unnamed delete `MudIconButton`.
- `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageTypeEditorComponent.razor:5-21` -- `MudMenu` activator `MudIconButton` in a `MudTooltip`; items are `MudMenuItem Label=@damageType.ToReadableString()` in a popover outside the component.
- `src/UI/SharedComponents/Inputs/RegexField.razor(.cs)` -- `MudTextField` with `Immediate=true`, `Error`/`ErrorText`; the model updates only on a valid value; `Validate()` returns the last validity. Read-only for this story.
- `src/Domain/CharacterSheet/Dices/Validation/DiceRollTemplateValidator.cs:32` -- `MatchRegex`, used by both dice fields. No unit test covers it.
- `uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/MartialAttacks/Pages/TestableAttackEditorPage.cs` -- `MainInfo()` uses `form:visible`; Save/Cancel use `:visible` CSS and return `Task`.
- `uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/MartialAttacks/Components/TestableAttackListEditor.cs:121` -- `OpenedAttackEditor()` waits on text "Main Informations".
- `uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/Characters/Pages/TestableCharacterEditorPage.cs:85-96` -- reference `Region(name)` pattern.
- `uitests/Framework/Components/ComponentObject.cs:42` -- `GetAccessibleNameState` parses `"{subject}: {state}"`.
- `uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/MartialAttacks/Components/AttackListEditorUiTests.cs` -- reference test shape, `StoredAttackNames` pattern.
- `tests/Domain/DnDEntitiesTests/Dices/DiceRollTemplateExpressionTests.cs` -- canonical expression ordering; place the new validator test next to it.

## Tasks & Acceptance

**Execution — production accessible names (pre-approved):**
- [x] `src/UI/CharacterSheetBlazorComponents/MartialAttacks/Components/AttackMainInfoEditor.razor` -- `role="region" aria-label="Main Informations"` on the `MudPaper`.
- [x] `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageRollTemplateCollectionEditorComponent.razor` -- `role="region" aria-label="Damages"` on the `MudPaper`; `aria-label="Add damage"` on the add button; wrap each row in `role="group" aria-label="Damage {n}"` (1-based index).
- [x] `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageRollTemplateEditorComponent.razor` -- `Label="Dice"` on the dice field; `aria-label="Delete damage"` on the delete button.
- [x] `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageTypeEditorComponent.razor` -- `aria-label="Damage type: {readable type}"` on the activator button.

**Execution — production changes added during implementation (approved by Benoit, 2026-10-25):**
- [x] `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageTypeEditorComponent.razor` -- `role="option"` on each `MudMenuItem` (MudBlazor 9 renders them role-less inside a `role="listbox"`).
- [x] Regex per type (approved by Benoit 2026-10-26): `src/UI/SharedComponents/Inputs/RegexField.razor.cs` -- check input with `Value!.IsValid(value)` (the bound type's own regex) instead of always `DiceRollTemplateValidator.MatchRegex`. The dice regex rejected stored to-hit values such as `-1` (blocking Save) and accepted `1d4`, which then crashed `DiceRollModifiersTemplate.ParseExpression`. Tests: `Should_Save_An_Unchanged_Negative_ToHit_Modifier`, `Should_Refuse_A_Dice_Term_In_ToHit_Modifiers` (both fail on the old check); unit `DiceRollModifiersTemplateRegexTests`.
- [x] Risk A (approved by Benoit 2026-10-26): `DamageTypeEditorComponent.razor` -- use `MudMenu`'s own `Icon`/`AriaLabel` button instead of `ActivatorContent`, with `MudTooltip` around the menu. `ActivatorContent` wrapped the button in a second `role="button"` with the same name, which forced a nested locator in `TestableDamageTypeEditorComponent`.
- [x] `src/UI/SharedComponents/Inputs/RegexField.razor(.cs)` -- pass the regex check to `MudTextField` through `Validation` instead of `Error`/`ErrorText`. Before, `MudTextField`'s own validation reset the error, so invalid input showed nothing. The error now renders and is linked through `aria-invalid` + `aria-describedby`. The regex runs once per change in `OnInputChangedAsync`; `Validation` returns that cached result (MudBlazor validates after `ValueChanged`).
- [x] `src/Domain/CharacterSheet/Dices/Mapping/MappingConfigurations.cs` (+ registration in `IoC/ServiceCollectionExtension.cs`) -- `MapWith` configs so `Copy`/`Clone` build new `DiceRollTemplate`/`DiceRollModifiersTemplate` instances. Before, the edit copy shared them with the original, so attack Cancel did not undo dice or to-hit edits (found by DAMAGE_CANCEL). Unit tests: `tests/Domain/DnDEntitiesTests/Dices/Mapping/MappingConfigurationsTests.cs`.
- [x] Review fix (approved by Benoit 2026-10-26): `src/Domain/CharacterSheet/Dices/Mapping/MappingConfigurations.cs` -- the `DiceRollTemplate` config uses a new copy constructor (`DiceRollTemplate.cs`) that copies `_dicesToRoll` (new `Dices` objects), `_wildcards` and `_staticModifier` instead of re-parsing `Expression`. Re-parsing threw for valid states whose text does not parse back: a zero total (`0`, `1-1`) and a negative leading term (`2-3` → `-1`, `1d6-2d6` → `-1d6`). `src/Domain/CharacterSheet/Dices/DiceRollTemplate.cs` -- `CreateExpression` returns `0` for an empty state instead of reading `expression[0]` on an empty string. Tests: `Should_Copy_A_Template_Whose_Expression_Does_Not_Reparse`, `Should_Not_Share_Dices_When_Copied`, `Should_Build_Zero_When_Empty`.
- [x] DW-024 (approved by Benoit 2026-10-26): `src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageRollTemplateCollectionEditorComponent.razor(.cs)` -- `@key` on each row; `Remove` drops the component reference. `src/UI/SharedComponents/Inputs/RegexField.razor.cs` -- resync when bound to a different object, not when the text is empty (the old rule also put the stored value back after the user emptied the field). Tests: `Should_Display_The_Remaining_Damage_After_Deleting_The_First`, `Should_Keep_Empty_Dice_After_Refused_Save`; `TestableDamageRollTemplateCollectionEditorComponent.WaitForDamageCount`.

**Execution — typed objects** (under `uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/`):
- [x] `MartialAttacks/Pages/TestableAttackEditorPage.cs` -- `MainInfo()` and new `Damages()` scope to their regions; Save/Cancel locate by role + name and return `TestableCharacterEditorPage`; add `AttemptSave()` (clicks Save, no navigation wait).
- [x] `MartialAttacks/Components/TestableAttackListEditor.cs` -- `OpenedAttackEditor()` waits on the "Main Informations" region.
- [x] `MartialAttacks/Components/TestableAttackMainInfoEditor.cs` -- Set/Get to-hit modifiers; `WaitForToHitModifiersError(message)` (was `GetToHitModifiersError()`; see the Playwright upgrade below).
- [x] `Damage/Components/TestableDamageRollTemplateCollectionEditorComponent.cs` -- `AddDamage()`, `Damage(number)` → row object, `GetDamageCount()`.
- [x] `Damage/Components/TestableDamageRollTemplateEditorComponent.cs` -- Set/Get dice, `WaitForDiceError(message)`, `Delete()`, `DamageType()` → type object.
- [x] Risk B (approved by Benoit 2026-10-26): `Microsoft.Playwright` 1.38.0 → 1.64.1 in all 8 `/uitests` projects; Chromium reinstalled with `playwright.ps1 install chromium`. Both error methods use `ToHaveAccessibleDescriptionAsync` (1.44+) instead of a copied `aria-describedby` script.
- [x] `Damage/Components/TestableDamageTypeEditorComponent.cs` -- `SetDamageType(readable)` (menu items located from `Page` by `menuitem` role; implemented with the `option` role, see production changes above), `GetDamageType()` via `GetAccessibleNameState`.

**Execution — tests** (under `uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/`, each fixture seeds and navigates in its own `[SetUp]` or test):
- [x] `MartialAttacks/Components/AttackMainInfoEditorUiTests.cs` -- `EditToHitModifiers` (TO_HIT, TO_HIT_INVALID), `DisplayToHitModifiers` (to-hit part of SEEDED_RENDERING).
- [x] `Damage/Components/DamageRollTemplateCollectionEditorComponentUiTests.cs` -- `AddDamages`, `DeleteDamages`, `DiscardDamageChanges` (DAMAGE_CANCEL), `DisplayDamages` (damage part of SEEDED_RENDERING).
- [x] `Damage/Components/DamageRollTemplateEditorComponentUiTests.cs` -- `EditDamageDice` (DAMAGE_DICE, DAMAGE_DICE_INVALID).
- [x] `Damage/Components/DamageTypeEditorComponentUiTests.cs` -- `ChangeDamageType` (DAMAGE_TYPE).
- [x] `tests/Domain/DnDEntitiesTests/Dices/Validation/DiceRollTemplateValidatorTests.cs` -- unit test `MatchRegex`: accepts a dice expression and a modifiers expression, rejects `abc`.

**Execution — close-out:**
- [x] `_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md` -- add Current map rows and an "Attack editor deep fields" coverage row; replace "Deep attack editor fields remain story 5c."
- [x] `_bmad-output/implementation-artifacts/deferred-work.md` -- note on closed DW-015 that the 5c remainder is resolved.
- [x] `_bmad-output/specs/spec-005-ui-test-navigation-library/.memlog.md` -- record build and per-project test counts.

**Acceptance Criteria:**
- Given a matrix action row, when typed controls change a field and the attack and parent are saved or canceled, then the real repository holds the expected value.
- Given independently seeded values, when the attack editor opens, then typed getters read them without editing.
- Given the damage type changes, when it is read back, then the activator's accessible name carries the new type.
- Given the full `/uitests` suite and `DnDEntitiesTests`, when they run, then every test passes.

## Design Notes

Invalid flow: `AttemptSave()` then read the error, then `Cancel()`. If Save wrongly navigated, the Cancel click finds no button and the test fails, so no absence wait is needed. Read the error through the textbox's `aria-invalid` and accessible description; if MudBlazor 9 does not link `ErrorText` to the input, HALT (Ask First) rather than locate the helper by CSS class.

A row group around `DamageRollTemplateEditorComponent` keeps the delete button and dice field unique within the row, so `Damage(2).Delete()` never relies on `.First`/`Nth`.

## Verification

**Commands:**
- `dotnet build DnDFightTool.CrossPlatform.slnf` -- expected: SUCCESS.
- `dotnet test tests/Domain/DnDEntitiesTests/DnDEntitiesTests.csproj` -- expected: green, new validator test included.
- `dotnet test` on `uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests`, `uitests/Tests/Components/DndUi.SharedUiTests`, `uitests/Tests/Framework/UiTestFrameworkUiTests` -- expected: all green.

**Manual checks:**
- New test files contain no `GetByRole`/`Locator`/`GotoAsync`/URL string.

## Suggested Review Order

**Cancel bug fix (domain mapping, outside the planned scope)**

- Entry point: Copy builds new dice objects by copying fields, so attack Cancel truly discards edits.
  [`MappingConfigurations.cs:11`](../../../../src/Domain/CharacterSheet/Dices/Mapping/MappingConfigurations.cs#L11)

- Empty state builds `0` instead of crashing the getter.
  [`DiceRollTemplate.cs:177`](../../../../src/Domain/CharacterSheet/Dices/DiceRollTemplate.cs#L177)

- Registered with the other character-sheet mappings; applies to every dice object app-wide.
  [`ServiceCollectionExtension.cs:16`](../../../../src/Domain/CharacterSheet/IoC/ServiceCollectionExtension.cs#L16)

**Validation error display (RegexField change)**

- The regex check moves into MudBlazor `Validation`, so the error shows and links to the field.
  [`RegexField.razor:13`](../../../../src/UI/SharedComponents/Inputs/RegexField.razor#L13)

- One check feeds both the field error and the Save gate.
  [`RegexField.razor.cs:38`](../../../../src/UI/SharedComponents/Inputs/RegexField.razor.cs#L38)

**Accessible names in production markup**

- Rows named by position, so a row is found without `.First`/`Nth`; `@key` keeps each row's component with its template (DW-024).
  [`DamageRollTemplateCollectionEditorComponent.razor:25`](../../../../src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageRollTemplateCollectionEditorComponent.razor#L25)

- Type carried in the button's name; `role="option"` added because MudBlazor 9 items have no role.
  [`DamageTypeEditorComponent.razor:8`](../../../../src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageTypeEditorComponent.razor#L8)

- Visible "Dice" label names the textbox; delete button named.
  [`DamageRollTemplateEditorComponent.razor:9`](../../../../src/UI/CharacterSheetBlazorComponents/Damage/Components/DamageRollTemplateEditorComponent.razor#L9)

- Main-info panel becomes a named region.
  [`AttackMainInfoEditor.razor:9`](../../../../src/UI/CharacterSheetBlazorComponents/MartialAttacks/Components/AttackMainInfoEditor.razor#L9)

**Typed objects**

- Save/Cancel return the character editor; `AttemptSave` supports the refused-save tests.
  [`TestableAttackEditorPage.cs:48`](../../../../uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/MartialAttacks/Pages/TestableAttackEditorPage.cs#L48)

- Activator is MudMenu's own button, found directly by its accessible name (Risk A).
  [`TestableDamageTypeEditorComponent.cs:26`](../../../../uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/Damage/Components/TestableDamageTypeEditorComponent.cs#L26)

- Error read with `ToHaveAccessibleDescriptionAsync` (Playwright 1.64.1, Risk B); same in the to-hit object.
  [`TestableDamageRollTemplateEditorComponent.cs:51`](../../../../uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/Damage/Components/TestableDamageRollTemplateEditorComponent.cs#L51)

**Tests**

- Cancel after a valid dice edit: the test that found the mapping bug.
  [`DamageRollTemplateCollectionEditorComponentUiTests.cs:200`](../../../../uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/Damage/Components/DamageRollTemplateCollectionEditorComponentUiTests.cs#L200)

- Refused save: error read, then Cancel proves the editor stayed open.
  [`DamageRollTemplateEditorComponentUiTests.cs:70`](../../../../uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/Damage/Components/DamageRollTemplateEditorComponentUiTests.cs#L70)

- To-hit save, refuse and discard (discard added at review).
  [`AttackMainInfoEditorUiTests.cs:1`](../../../../uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/MartialAttacks/Components/AttackMainInfoEditorUiTests.cs#L1)

- Unit tests: nested attack copy owns its dice; regex accept/reject.
  [`MappingConfigurationsTests.cs:1`](../../../../tests/Domain/DnDEntitiesTests/Dices/Mapping/MappingConfigurationsTests.cs#L1)
  [`DiceRollTemplateValidatorTests.cs:1`](../../../../tests/Domain/DnDEntitiesTests/Dices/Validation/DiceRollTemplateValidatorTests.cs#L1)
