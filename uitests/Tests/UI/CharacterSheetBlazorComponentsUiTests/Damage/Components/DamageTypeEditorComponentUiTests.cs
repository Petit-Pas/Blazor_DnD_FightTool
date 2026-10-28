using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.MartialAttacks.Pages;
using DomainTestsUtilities.Factories.Characters;
using DomainTestsUtilities.Factories.Damage;
using DomainTestsUtilities.Factories.MartialAttacks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.Damage.Components;

/// <summary>
///     Damage-type picking on one damage-roll row of the attack editor.
/// </summary>
public static class DamageTypeEditorComponentUiTests
{
    /// <summary>
    ///     Changing a damage roll's type.
    /// </summary>
    public sealed class ChangeDamageType : IsolatedScenarioFixture
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
        ///     Picking Cold shows it on the picker and stores it once the attack and its character are saved.
        /// </summary>
        [Test]
        public async Task Should_Save_Damage_Type()
        {
            // Act
            var damageType = _attackEditor.Damages().Damage(1).DamageType();
            await damageType.SetDamageType("Cold");
            await CaptureAsync("before-save");
            var editor = await _attackEditor.Save();
            await editor.Save();

            // Assert
            _repository.GetAllCharacters().Single().MartialAttacks.Values.Single()
                .Damages.Should().ContainSingle()
                .Which.Type.Should().Be(DamageTypeEnum.Cold);
        }
    }
}
