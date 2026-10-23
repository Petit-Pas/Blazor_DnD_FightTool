using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.AbilityScores;

/// <summary>
///     Per-ability score, save bonus, save mastery and mastery bonus editing on the character editor's
///     "Abilities &amp; Skills" tab.
/// </summary>
public static class AbilityScoresEditorComponentUiTests
{
    /// <summary>
    ///     Rendering ability score data seeded through the repository.
    /// </summary>
    public sealed class DisplayAbilityScores : IsolatedScenarioFixture
    {
        /// <summary>
        ///     The ability editor renders the stored score, save bonus, save-mastery state and mastery bonus.
        /// </summary>
        [Test]
        public async Task Should_Display_Seeded_Ability_Fields()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services, character =>
            {
                var strength = character.AbilityScores.First(ability => ability.Ability == AbilityEnum.Strength);
                strength.Score = 16;
                strength.ArbitrarySaveModifier = 3;
                strength.HasMastery = true;
                character.AbilityScores.MasteryBonus = 4;
            });

            // Act
            var abilities = await editor.OpenAbilityScores();
            var strength = abilities.Ability("STR");

            // Assert
            (await strength.GetScore()).Should().Be(16);
            (await strength.GetSaveBonus()).Should().Be(3);
            (await strength.HasSaveMastery()).Should().BeTrue();
            (await abilities.GetMasteryBonus()).Should().Be(4);
        }
    }

    /// <summary>
    ///     Saving or canceling edits to a single ability's fields and to the shared mastery bonus.
    /// </summary>
    public sealed class EditAbilityScores : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Saving updates the ability's raw score in the repository.
        /// </summary>
        [Test]
        public async Task Should_Save_Ability_Score()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services);
            var abilities = await editor.OpenAbilityScores();
            var strength = abilities.Ability("STR");

            // Act
            await strength.SetScore(18);
            await editor.Save();

            // Assert
            StoredStrength(Services).Score.Should().Be(18);
        }

        /// <summary>
        ///     Saving updates the ability's arbitrary save bonus in the repository.
        /// </summary>
        [Test]
        public async Task Should_Save_Save_Bonus()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services);
            var abilities = await editor.OpenAbilityScores();
            var strength = abilities.Ability("STR");

            // Act
            await strength.SetSaveBonus(2);
            await editor.Save();

            // Assert
            StoredStrength(Services).ArbitrarySaveModifier.Should().Be(2);
        }

        /// <summary>
        ///     Saving updates the shared mastery bonus applied to every mastered ability in the repository.
        /// </summary>
        [Test]
        public async Task Should_Save_Mastery_Bonus()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services);
            var abilities = await editor.OpenAbilityScores();

            // Act
            await abilities.SetMasteryBonus(5);
            await editor.Save();

            // Assert
            StoredAbilityScores(Services).MasteryBonus.Should().Be(5);
        }

        /// <summary>
        ///     Saving updates the ability's save-mastery state after toggling its chip.
        /// </summary>
        [Test]
        public async Task Should_Save_Save_Mastery_Toggle()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services);
            var abilities = await editor.OpenAbilityScores();
            var strength = abilities.Ability("STR");

            // Act
            await strength.ToggleSaveMastery();
            await editor.Save();

            // Assert
            StoredStrength(Services).HasMastery.Should().BeTrue();
        }

        /// <summary>
        ///     Canceling an ability-score edit preserves the repository's original score.
        /// </summary>
        [Test]
        public async Task Should_Discard_Changes_On_Cancel()
        {
            // Arrange
            var editor = await OpenCharacter(Page, BaseUrl, Services);
            var abilities = await editor.OpenAbilityScores();
            var strength = abilities.Ability("STR");

            // Act
            await strength.SetScore(25);
            await editor.Cancel();

            // Assert
            StoredStrength(Services).Score.Should().Be(10);
        }
    }

    private static AbilityScore StoredStrength(IServiceProvider services)
    {
        return StoredAbilityScores(services).First(ability => ability.Ability == AbilityEnum.Strength);
    }

    private static AbilityScoresCollection StoredAbilityScores(IServiceProvider services)
    {
        return services.GetRequiredService<ICharacterRepository>().GetAllCharacters().Single().AbilityScores;
    }

    private static async Task<TestableCharacterEditorPage> OpenCharacter(
        IPage page, string baseUrl, IServiceProvider services, Action<Character>? configure = null)
    {
        var character = CharacterFactory.BuildPlayer(name: "Fighter");
        character.AbilityScores = new AbilityScoresCollection(withDefaults: true);
        configure?.Invoke(character);

        services.GetRequiredService<ICharacterRepository>().Save(character);
        var list = new TestableCharacterListEditorPage(page, baseUrl);
        await list.GoToAsync();
        return await list.EditPlayer("Fighter");
    }
}
