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
///     Dice editing on one damage-roll row of the attack editor.
/// </summary>
public static class DamageRollTemplateEditorComponentUiTests
{
    /// <summary>
    ///     Saving and refusing dice expression changes.
    /// </summary>
    public sealed class EditDamageDice : IsolatedScenarioFixture
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
        ///     A valid dice expression is stored once the attack and its character are saved.
        /// </summary>
        [Test]
        public async Task Should_Save_Dice()
        {
            // Act
            await _attackEditor.Damages().Damage(1).SetDice("2d8+STR");
            await CaptureAsync("before-save");
            var editor = await _attackEditor.Save();
            await editor.Save();

            // Assert
            StoredAttack(_repository).Damages.Should().ContainSingle()
                .Which.Dices.Expression.Should().Be("2d8+STR");
        }

        /// <summary>
        ///     An invalid dice expression shows an error, blocks Save, and leaves the stored value unchanged.
        /// </summary>
        [Test]
        public async Task Should_Refuse_Invalid_Dice()
        {
            // Act
            var damage = _attackEditor.Damages().Damage(1);
            await damage.SetDice("abc");
            await _attackEditor.AttemptSave();
            await damage.WaitForDiceError("Invalid dice throw template, should match something like 2d8+1d4+2+STR.");
            await CaptureAsync("error-shown");
            await _attackEditor.ExpectStillOpen();
            await _attackEditor.Cancel();
            await _editor.Save();

            // Assert
            StoredAttack(_repository).Damages.Should().ContainSingle()
                .Which.Dices.Expression.Should().Be("1d6");
        }

        /// <summary>
        ///     An emptied dice field keeps its text and error after a refused Save re-renders the page.
        /// </summary>
        [Test]
        public async Task Should_Keep_Empty_Dice_After_Refused_Save()
        {
            // Act
            var damage = _attackEditor.Damages().Damage(1);
            await damage.SetDice("");
            await _attackEditor.AttemptSave();
            await damage.WaitForDiceError("Cannot be empty");
            await CaptureAsync("error-shown");
            var dice = await damage.GetDice();
            await _attackEditor.ExpectStillOpen();
            await _attackEditor.Cancel();
            await _editor.Save();

            // Assert
            dice.Should().BeEmpty();
            StoredAttack(_repository).Damages.Should().ContainSingle()
                .Which.Dices.Expression.Should().Be("1d6");
        }
    }

    private static MartialAttackTemplate StoredAttack(ICharacterRepository repository)
    {
        return repository.GetAllCharacters().Single().MartialAttacks.Values.Single();
    }
}
