using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.MartialAttacks.Pages;
using DomainTestsUtilities.Factories.Characters;
using DomainTestsUtilities.Factories.Damage;
using DomainTestsUtilities.Factories.MartialAttacks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.Damage.Components;

/// <summary>
///     Adding, deleting and displaying damage rolls in the attack editor's "Damages" panel.
/// </summary>
public static class DamageRollTemplateCollectionEditorComponentUiTests
{
    /// <summary>
    ///     Rendering damage rolls seeded through the repository.
    /// </summary>
    public sealed class DisplayDamages : IsolatedScenarioFixture
    {
        private ICharacterRepository _repository = null!;
        private TestableCharacterListEditorPage _characterList = null!;

        /// <summary>
        ///     Resolves the repository and builds the list page object.
        /// </summary>
        [SetUp]
        public void SetUpCharacterLibrary()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            _characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
        }

        /// <summary>
        ///     The attack editor renders one row per stored damage roll, each with its own type and dice.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_Damages()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter");
            fighter.MartialAttacks.Add(MartialAttackTemplateFactory.Build(
                name: "Longsword",
                damages: DamageRollTemplateFactory.BuildCollection(
                [
                    DamageRollTemplateFactory.Build("1d6", DamageTypeEnum.Fire),
                    DamageRollTemplateFactory.Build("2d8", DamageTypeEnum.Cold)
                ])));
            _repository.Save(fighter);
            await _characterList.GoToAsync();
            var editor = await _characterList.EditPlayer("Fighter");
            var attacks = await editor.OpenAttacks();

            // Act
            var attackEditor = await attacks.EditAttack("Longsword");

            // Assert
            var damages = attackEditor.Damages();
            (await damages.GetDamageCount()).Should().Be(2);
            (await damages.Damage(1).DamageType().GetDamageType()).Should().Be("Fire");
            (await damages.Damage(1).GetDice()).Should().Be("1d6");
            (await damages.Damage(2).DamageType().GetDamageType()).Should().Be("Cold");
            (await damages.Damage(2).GetDice()).Should().Be("2d8");
        }
    }

    /// <summary>
    ///     Adding a damage roll to an attack.
    /// </summary>
    public sealed class AddDamages : IsolatedScenarioFixture
    {
        private ICharacterRepository _repository = null!;
        private TestableAttackEditorPage _attackEditor = null!;

        /// <summary>
        ///     Seeds a player with one attack dealing Fire <c>1d6</c> and opens that attack's editor.
        /// </summary>
        [SetUp]
        public async Task OpenAttackEditor()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter");
            fighter.MartialAttacks.Add(MartialAttackTemplateFactory.Build(
                name: "Longsword",
                damages: DamageRollTemplateFactory.BuildCollection([DamageRollTemplateFactory.Build("1d6", DamageTypeEnum.Fire)])));
            _repository.Save(fighter);
            var characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
            await characterList.GoToAsync();
            var editor = await characterList.EditPlayer("Fighter");
            var attacks = await editor.OpenAttacks();
            _attackEditor = await attacks.EditAttack("Longsword");
        }

        /// <summary>
        ///     Adding a damage roll stores a second, default roll and keeps the first one unchanged.
        /// </summary>
        [Test]
        public async Task Should_Add_A_Damage()
        {
            // Act
            await _attackEditor.Damages().AddDamage();
            await CaptureAsync("before-save");
            var editor = await _attackEditor.Save();
            await editor.Save();

            // Assert
            var damages = StoredAttack(_repository).Damages;
            damages.Should().HaveCount(2);
            damages[0].Type.Should().Be(DamageTypeEnum.Fire);
            damages[0].Dices.Expression.Should().Be("1d6");
            damages[1].Type.Should().Be(DamageTypeEnum.Bludgeoning);
            damages[1].Dices.Expression.Should().Be("1d6+STR");
        }
    }

    /// <summary>
    ///     Deleting one damage roll from an attack.
    /// </summary>
    public sealed class DeleteDamages : IsolatedScenarioFixture
    {
        private ICharacterRepository _repository = null!;
        private TestableAttackEditorPage _attackEditor = null!;

        /// <summary>
        ///     Seeds a player with one attack dealing Fire <c>1d6</c> and Cold <c>2d8</c> and opens that attack's editor.
        /// </summary>
        [SetUp]
        public async Task OpenAttackEditor()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter");
            fighter.MartialAttacks.Add(MartialAttackTemplateFactory.Build(
                name: "Longsword",
                damages: DamageRollTemplateFactory.BuildCollection(
                [
                    DamageRollTemplateFactory.Build("1d6", DamageTypeEnum.Fire),
                    DamageRollTemplateFactory.Build("2d8", DamageTypeEnum.Cold)
                ])));
            _repository.Save(fighter);
            var characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
            await characterList.GoToAsync();
            var editor = await characterList.EditPlayer("Fighter");
            var attacks = await editor.OpenAttacks();
            _attackEditor = await attacks.EditAttack("Longsword");
        }

        /// <summary>
        ///     Deleting the first damage roll stores only the second one.
        /// </summary>
        [Test]
        public async Task Should_Delete_Only_The_Selected_Damage()
        {
            // Act
            await _attackEditor.Damages().Damage(1).Delete();
            await _attackEditor.Damages().WaitForDamageCount(1);
            await CaptureAsync("before-save");
            var editor = await _attackEditor.Save();
            await editor.Save();

            // Assert
            var damage = StoredAttack(_repository).Damages.Should().ContainSingle().Subject;
            damage.Type.Should().Be(DamageTypeEnum.Cold);
            damage.Dices.Expression.Should().Be("2d8");
        }

        /// <summary>
        ///     UI-local display: after deleting the first row, the remaining row shows its own type and dice (DW-024).
        /// </summary>
        [Test]
        public async Task Should_Display_The_Remaining_Damage_After_Deleting_The_First()
        {
            // Arrange
            var damages = _attackEditor.Damages();

            // Act
            await damages.Damage(1).Delete();
            await damages.WaitForDamageCount(1);

            // Assert
            var remaining = damages.Damage(1);
            await remaining.DamageType().WaitForDamageType("Cold");
            await remaining.WaitForDice("2d8");
        }

        /// <summary>
        ///     The remaining row keeps its own unsaved invalid text after the first row is deleted (pins <c>@key</c> on the rows).
        /// </summary>
        [Test]
        public async Task Should_Keep_Invalid_Dice_On_The_Remaining_Damage_After_Deleting_The_First()
        {
            // Arrange
            var damages = _attackEditor.Damages();
            await damages.Damage(2).SetDice("abc");

            // Act
            await damages.Damage(1).Delete();
            await damages.WaitForDamageCount(1);

            // Assert
            var remaining = damages.Damage(1);
            await remaining.WaitForDiceError("Invalid dice throw template, should match something like 2d8+1d4+2+STR.");
            await remaining.WaitForDice("abc");
        }

        /// <summary>
        ///     A deleted row with invalid dice does not block Save.
        /// </summary>
        [Test]
        public async Task Should_Save_After_Deleting_A_Damage_With_Invalid_Dice()
        {
            // Act
            var damages = _attackEditor.Damages();
            await damages.Damage(1).SetDice("abc");
            await damages.Damage(1).Delete();
            await damages.WaitForDamageCount(1);
            await CaptureAsync("before-save");
            var editor = await _attackEditor.Save();
            await editor.Save();

            // Assert
            var damage = StoredAttack(_repository).Damages.Should().ContainSingle().Subject;
            damage.Type.Should().Be(DamageTypeEnum.Cold);
            damage.Dices.Expression.Should().Be("2d8");
        }
    }

    /// <summary>
    ///     Canceling the attack edit discards damage changes.
    /// </summary>
    public sealed class DiscardDamageChanges : IsolatedScenarioFixture
    {
        private ICharacterRepository _repository = null!;
        private TestableCharacterEditorPage _editor = null!;
        private TestableAttackEditorPage _attackEditor = null!;

        /// <summary>
        ///     Seeds a player with one attack dealing Fire <c>1d6</c> and opens that attack's editor.
        /// </summary>
        [SetUp]
        public async Task OpenAttackEditor()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter");
            fighter.MartialAttacks.Add(MartialAttackTemplateFactory.Build(
                name: "Longsword",
                damages: DamageRollTemplateFactory.BuildCollection([DamageRollTemplateFactory.Build("1d6", DamageTypeEnum.Fire)])));
            _repository.Save(fighter);
            var characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
            await characterList.GoToAsync();
            _editor = await characterList.EditPlayer("Fighter");
            var attacks = await _editor.OpenAttacks();
            _attackEditor = await attacks.EditAttack("Longsword");
        }

        /// <summary>
        ///     A dice change followed by Cancel leaves the stored dice unchanged.
        /// </summary>
        [Test]
        public async Task Should_Discard_Damage_Changes_On_Cancel()
        {
            // Act
            await _attackEditor.Damages().Damage(1).SetDice("2d8");
            await CaptureAsync("before-cancel");
            await _attackEditor.Cancel();
            await _editor.Save();

            // Assert
            StoredAttack(_repository).Damages.Should().ContainSingle()
                .Which.Dices.Expression.Should().Be("1d6");
        }
    }

    private static MartialAttackTemplate StoredAttack(ICharacterRepository repository)
    {
        return repository.GetAllCharacters().Single().MartialAttacks.Values.Single();
    }
}
