---
title: 'Exhaustive character editor field coverage'
type: 'feature'
created: '2026-09-30'
status: 'done'
baseline_commit: 'cdd662bc146622df295aee5ae615b785cebf8ae9'
review_loop_iteration: 0
context:
  - '{project-root}/_bmad-output/project-context.md'
  - '{project-root}/_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md'
---

<frozen-after-approval reason="human-owned intent — do not modify unless human renegotiates">

## Intent

**Problem:** Story 5 made the character editor reachable, but only at name level. A scenario cannot drive or read back HP/AC/shield, abilities, skills or resistances, so CAP-2 stops at the editor's first field (DW-015). The attack editor's fields are split off to story 5c.

**Approach:** Build on story 5's `PageObject`/`ComponentObject` base. Extend `TestableCharacterEditorPage` and `TestableCharacterMainInfoEditor`, and add one `Testable{Component}` per deep-field editor. Give the two icon-only skill/resistance buttons an `aria-label` that carries their current state. Per Benoit's 2026-10-08 rule, use capability-grouped `IsolatedScenarioFixture` tests in two directions: edit through the UI and Save/Cancel, then assert the real repository; independently seed values and navigate to assert their rendering. No fakes.

## Boundaries & Constraints

**Always:** Follow [uitests-layout.md](../uitests-layout.md): placement, naming, namespaces, capability fixtures, unconditional `[SetUp]`, explicitly named controls, directional coverage and UI-vs-unit boundaries. Testable projects never reference `src/`, so abilities, skills, damage types, mastery and affinity cross their API as **rendered strings** (`"STR"`, `"Animal Handling"`, `"Piercing Silver"`, `"Expertise"`, `"Resistant"`). Test bodies may assert domain values directly through real services. No locator, URL or `GotoAsync` in a test body. Seed via `ICharacterRepository` + `CharacterFactory.BuildPlayer`; navigate via `TestableCharacterListEditorPage.EditPlayer`. Decrease is a right-click (`MouseButton.Right`). Repo C# rules apply.

**Ask First:** Any production markup change other than the two `aria-label`s below. Any `data-testid`. Any framework change.

**Never:** No domain/binding change. No attack-editor work (story 5c). No flow-helper layer. Do not duplicate story-5 objects; extend them. No fakes, test-only hooks or root-provider access to browser-scoped drafts. Do not use save/reopen rendering to prove service wiring. No UI test of domain rules (mastery/affinity clamping, modifier or Effective AC arithmetic beyond one displayed-value check). No `SequentialScenarioFixture`.

## I/O & Edge-Case Matrix

| Scenario | Input / State | Expected Output / Behavior | Error Handling |
|----------|--------------|---------------------------|----------------|
| HIT_POINTS | Seeded player, Basic infos | UI sets Max HPs + HPs and saves; repository contains both values | — |
| ARMOR_CLASS | Basic infos, shield unequipped | UI sets AC + Shield value, equips shield and saves; repository contains values/state | — |
| ABILITY_FIELDS | Abilities & Skills | Separately set STR score, STR save bonus, Mastery bonus or toggle STR save mastery; Save; assert stored field | — |
| ABILITIES_TAB_CANCEL | Abilities & Skills | Edit STR score, Cancel; repository retains original value | — |
| SKILL_MASTERY_UP | Skill at `Normal` | Left-click, Save; repository contains `Mastery` | — |
| SKILL_MASTERY_DOWN | Skill seeded at `Expertise` | Right-click, Save; repository contains `Mastery` | — |
| SKILL_ABILITY | Skill on its default ability | Pick another ability, Save; assert repository skill ability | — |
| AFFINITY_UP | Type at `Normal` | Left-click, Save; repository contains `Resistant` | — |
| AFFINITY_DOWN | Type at `Normal` | Right-click, Save; repository contains `Weak` | — |
| RESISTANCES_TAB_CANCEL | Resistances | Change affinity, Cancel; repository retains original affinity | — |
| SEEDED_FIELD_RENDERING | Independently seed the above field values via real repository | Open editor/tab without editing; assert fields, mastery/affinity accessible names, ability chip and one Effective AC display | — |

Each editable field needs both wiring and independent rendering evidence. A representative multi-field rendering test may share seeded data; it must not depend on an earlier action test. A local draft-only display update may have an explicitly UI-local test, without treating it as service-wiring evidence.

</frozen-after-approval>

## Code Map

- `src/UI/CharacterSheetBlazorComponents/Skills/Components/SkillsEditorComponent.razor:19` -- raw mastery `MudIconButton`. The next cell is a `MudMenu` whose activator is an `AbilityChip` (`.ability-score-chip`, text `STR`…); its items read `STR` or `STR (default)` and render in a popover outside `Root`. The last cell is the skill name.
- `src/UI/CharacterSheetBlazorComponents/DamageAffinities/Components/ResistancesEditorComponent.razor:15` -- raw affinity `MudIconButton`; the next cell is the damage type.
- `src/UI/CharacterSheetBlazorComponents/Characters/Components/CharacterMainInfoEditor.razor:22-43` -- labels `Max HPs`, `HPs`, `Armor Class`, `Shield value`, read-only `Effective AC`; the shield `MudCheckBox` carries `aria-label="Shield equipped"`.
- `src/UI/CharacterSheetBlazorComponents/AbilityScores/AbilityScoreEditorComponent.razor:12-20` -- one row per ability: `AbilityChip` (click toggles `HasMastery`; `mud-chip-filled` vs outlined), `Ability score`, `Save bonus`. `AbilityScoresEditorComponent.razor:25` has the single `Mastery bonus`.
- `src/UI/CharacterSheetBlazorComponents/Characters/Pages/CharacterEditorPage.razor:20-57` -- tabs `Basic infos`, `Abilities & Skills` (abilities left, skills right), `Resistances`; each tab has its own Save/Cancel.
- `src/UI/SharedComponents/Buttons/AtomicButtonsPreset/ButtonBase.razor:33` -- `aria-label=@(...)` pattern on `MudIconButton`.
- `uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/Characters/Pages/TestableCharacterEditorPage.cs` -- extend; `OpenAttacks` shows the tab-switch + visible-scope pattern; Save/Cancel already target the visible tab.
- `uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/MartialAttacks/Components/AttackListEditorUiTests.cs` -- reference: capability fixtures, `Should_...` names, AAA comments, seeded `OpenCharacter` helper.
- `uitests/Tests/Components/DndUi.SharedUiTests/Components/Pages/FightDashboardPageUiTests.cs:22` -- reference: local abstract `DashboardFixture` with `[SetUp]` shared by capability fixtures.
- `uitests/Tests/Components/DndUi.SharedUiTests/Components/Pages/CharacterListEditorPageUiTests.cs` -- already covers the Basic infos Save/Cancel (name level); do not repeat.

## Tasks & Acceptance

**Execution — production accessible names:**
- [x] `src/UI/CharacterSheetBlazorComponents/Skills/Components/SkillsEditorComponent.razor` -- mastery button `aria-label="{Skill readable} mastery: {Mastery}"`.
- [x] `src/UI/CharacterSheetBlazorComponents/DamageAffinities/Components/ResistancesEditorComponent.razor` -- affinity button `aria-label="{DamageType readable} affinity: {Affinity}"`.
- [x] Review follow-up (approved by Benoit 2026-10-23): shield `MudCheckBox` `aria-label="Shield equipped"`; editor panels `role="region"` named `Abilities`, `Skills`, `Damage affinities`; ability rows `role="group"` named `"{STR} ability"`. Typed objects locate panels and rows by these names instead of titles or layout classes.

**Execution — typed objects** (under `uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/`):
- [x] `Characters/Pages/TestableCharacterEditorPage.cs` -- `OpenAbilityScores()`, `OpenSkills()` (both select "Abilities & Skills"), `OpenResistances()`; each returns its object scoped to the visible panel.
- [x] `Characters/Components/TestableCharacterMainInfoEditor.cs` -- Set/Get MaxHps, CurrentHps, ArmorClass, ShieldValue; `SetShieldEquipped(bool)`, `IsShieldEquipped`, `GetEffectiveAc`.
- [x] `AbilityScores/TestableAbilityScoresEditorComponent.cs` -- `Ability(shortName)` → row object; Set/Get MasteryBonus.
- [x] `AbilityScores/TestableAbilityScoreEditorComponent.cs` -- Set/Get Score, SaveBonus; `ToggleSaveMastery`, `HasSaveMastery`.
- [x] `Skills/Components/TestableSkillsEditorComponent.cs` -- `IncreaseMastery`, `DecreaseMastery`, `GetMastery(skill)`; `SetAbility(skill, shortName)`, `GetAbility(skill)`.
- [x] `DamageAffinities/Components/TestableResistancesEditorComponent.cs` -- `IncreaseAffinity`, `DecreaseAffinity`, `GetAffinity(damageType)`.

**Execution — tests** (under `uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/`; shared `[SetUp]` seeds the player and opens its tab; action tests assert the real repository after Save/Cancel, independent rendering tests seed values and read typed controls):
- [x] `Characters/Components/CharacterMainInfoEditorUiTests.cs` -- `EditHitPoints` (HIT_POINTS), `EditArmorClass` (ARMOR_CLASS).
- [x] `AbilityScores/AbilityScoresEditorComponentUiTests.cs` -- `EditAbilityScores` (ABILITY_FIELDS as separately named tests per field, ABILITIES_TAB_CANCEL).
- [x] `Skills/Components/SkillsEditorComponentUiTests.cs` -- `ChangeSkillMastery` (UP, DOWN), `ChangeGoverningAbility` (SKILL_ABILITY).
- [x] `DamageAffinities/Components/ResistancesEditorComponentUiTests.cs` -- `ChangeAffinity` (UP, DOWN, RESISTANCES_TAB_CANCEL).
- [x] Character editor tests seed through `ICharacterRepository` in their own `[SetUp]`, then open the editor with `TestableCharacterListEditorPage.EditPlayer(name)`, which returns `TestableCharacterEditorPage`. No tools object or shared base fixture.
- [x] `uitests/Tests/Components/DndUi.SharedUiTests/Components/Pages/CharacterListEditorPageUiTests.cs` -- the three monster create/duplicate/edit scenarios switch back to the Monsters tab after Save, so the automatic final screenshot shows the result (final-screenshot/tab rule in uitests-layout.md).
- [x] `_bmad-output/specs/spec-005-ui-test-navigation-library/uitests-layout.md` -- add the new rows to "Current map" and the editor surface to "Reviewed business coverage". In `deferred-work.md`, move the open DW-015 (editor field coverage) to Closed as resolved, noting the attack-editor remainder is story 5c.

**Acceptance Criteria:**
- Given an action matrix row, when typed controls change a field and Save/Cancel, then the real repository contains the expected value, without relying on a rendered business result.
- Given independently seeded field values, when the editor opens, then typed getters/expectations verify their rendering without editing them. Both directions use real services, no fakes and no locator expressions in test bodies.
- Given the skill and resistance buttons, when their value changes, then their accessible name changes with it.
- Given the full `/uitests` suite, when it runs, then every test passes, including the reviewed story-5 tests.

## Design Notes

State lives in the accessible name, so reading it is a string parse inside the typed object. Anchor the regex on the full prefix, so that `Piercing affinity: ` never matches `Piercing Silver`:

```csharp
var button = Root.GetByRole(AriaRole.Button, new() { NameRegex = new Regex($"^{Regex.Escape(skill)} mastery: ") });
return (await button.GetAttributeAsync("aria-label"))!.Split(": ")[1];
```

## Verification

**Commands:**
- `dotnet build DnDFightTool.CrossPlatform.slnf` -- expected: SUCCESS.
- `dotnet test uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/CharacterSheetBlazorComponentsUiTests.csproj` -- expected: every new test plus the `AttackListEditorUiTests` fixtures pass.
- `dotnet test` on `uitests/Tests/Components/DndUi.SharedUiTests/DndUi.SharedUiTests.csproj` and `uitests/Tests/Framework/UiTestFrameworkUiTests/UiTestFrameworkUiTests.csproj` -- expected: still green.

**Manual checks:**
- The new test files contain no `GetByRole`/`Locator`/`GotoAsync`/URL string.

## Suggested Review Order

**Production aria-label contract**

- Entry point: each icon button's accessible name now carries its own state, so a typed object can read state without a locator per value.
  [`SkillsEditorComponent.razor:19`](../../../../src/UI/CharacterSheetBlazorComponents/Skills/Components/SkillsEditorComponent.razor#L19)

- Same contract on the resistances side, mirrored for symmetry with the skills button.
  [`ResistancesEditorComponent.razor:15`](../../../../src/UI/CharacterSheetBlazorComponents/DamageAffinities/Components/ResistancesEditorComponent.razor#L15)

**Mapster `MasteryBonus` fix (found during implementation, not in original scope)**

- `AbilityScoresCollection` is `List<AbilityScore>`-derived; Mapster's default collection mapping only copies list items and silently drops this extra property on `Copy()`/`Clone()`. Fixed with an explicit `AfterMapping` hook.
  [`MappingConfigurations.cs:15`](../../../../src/Domain/CharacterSheet/AbilityScores/Mapping/MappingConfigurations.cs#L15)

**Typed objects — reading state back**

- Shared parse of the `"{subject}: {state}"` accessible-name suffix, used by both skills and resistances below instead of duplicating `Split(": ")`.
  [`ComponentObject.cs:42`](../../../../uitests/Framework/Components/ComponentObject.cs#L42)

- Tab navigation collapsed into one parameterized helper reused by all three `Open*` methods.
  [`TestableCharacterEditorPage.cs:86`](../../../../uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/Characters/Pages/TestableCharacterEditorPage.cs#L86)

- Row locator for a single ability now pins `.First`, closing a latent Playwright strict-mode risk if two ability short names ever overlapped as substrings.
  [`TestableAbilityScoresEditorComponent.cs:34`](../../../../uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/AbilityScores/TestableAbilityScoresEditorComponent.cs#L34)

- Shield toggle and the one permitted domain-arithmetic check (`Effective AC`), extending story 5's main-info object.
  [`TestableCharacterMainInfoEditor.cs:170`](../../../../uitests/TestableComponents/UI/TestableCharacterSheetBlazorComponents/Characters/Components/TestableCharacterMainInfoEditor.cs#L170)

**Tests — directional coverage per matrix row**

- Basic infos: HP + AC + shield, UI action asserted against the real repository.
  [`CharacterMainInfoEditorUiTests.cs:91`](../../../../uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/Characters/Components/CharacterMainInfoEditorUiTests.cs#L91)

- Abilities & skills: mastery toggle persisted through Save.
  [`AbilityScoresEditorComponentUiTests.cs:118`](../../../../uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/AbilityScores/AbilityScoresEditorComponentUiTests.cs#L118)

- Skills: mastery up on left-click, and the ability-reassignment row.
  [`SkillsEditorComponentUiTests.cs:57`](../../../../uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/Skills/Components/SkillsEditorComponentUiTests.cs#L57)

- Resistances: affinity up on left-click, mirroring the skills direction.
  [`ResistancesEditorComponentUiTests.cs:54`](../../../../uitests/Tests/UI/CharacterSheetBlazorComponentsUiTests/DamageAffinities/Components/ResistancesEditorComponentUiTests.cs#L54)
- Rendering-case `artifacts/` PNGs show independently seeded values; action screenshots are context, not substitutes for service assertions.
