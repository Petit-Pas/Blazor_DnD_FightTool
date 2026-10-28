using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Skills;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Skills.Components;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.Skills.Components;

/// <summary>
///     Skill mastery and governing-ability editing on the character editor's "Abilities &amp; Skills" tab.
/// </summary>
public static class SkillsEditorComponentUiTests
{
    /// <summary>
    ///     Rendering skill mastery and governing-ability data seeded through the repository.
    /// </summary>
    public sealed class DisplaySkills : IsolatedScenarioFixture
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
        ///     The skills table renders the stored mastery, via the mastery button's accessible name, and the stored
        ///     governing ability, via the ability chip.
        /// </summary>
        [Test]
        public async Task Should_Display_Seeded_Mastery_And_Ability()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter", withDefaults: true);
            fighter.Skills[SkillEnum.Athletics].Mastery = SkillMasteryEnum.Expertise;
            fighter.Skills[SkillEnum.Athletics].Ability = AbilityEnum.Dexterity;
            _repository.Save(fighter);
            await _characterList.GoToAsync();
            var editor = await _characterList.EditPlayer("Fighter");

            // Act
            var skills = await editor.OpenSkills();

            // Assert
            (await skills.GetMastery("Athletics")).Should().Be("Expertise");
            (await skills.GetAbility("Athletics")).Should().Be("DEX");
        }
    }

    /// <summary>
    ///     Opens the skills panel of a seeded player whose Athletics is at Normal on Strength and whose Stealth is at
    ///     Expertise.
    /// </summary>
    public abstract class SkillsEditFixture : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Gets the real character repository.
        /// </summary>
        protected ICharacterRepository Repository { get; private set; } = null!;

        /// <summary>
        ///     Gets the opened character editor.
        /// </summary>
        protected TestableCharacterEditorPage Editor { get; private set; } = null!;

        /// <summary>
        ///     Gets the skills panel of the opened editor.
        /// </summary>
        protected TestableSkillsEditorComponent Skills { get; private set; } = null!;

        /// <summary>
        ///     Seeds the player and opens its skills panel.
        /// </summary>
        [SetUp]
        public async Task OpenSkillsPanel()
        {
            Repository = Services.GetRequiredService<ICharacterRepository>();
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter", withDefaults: true);
            fighter.Skills[SkillEnum.Athletics].Mastery = SkillMasteryEnum.Normal;
            fighter.Skills[SkillEnum.Athletics].Ability = AbilityEnum.Strength;
            fighter.Skills[SkillEnum.Stealth].Mastery = SkillMasteryEnum.Expertise;
            Repository.Save(fighter);
            var characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
            await characterList.GoToAsync();
            Editor = await characterList.EditPlayer("Fighter");
            Skills = await Editor.OpenSkills();
        }

        /// <summary>
        ///     Reads the stored state of <paramref name="skill"/>.
        /// </summary>
        /// <param name="skill">The skill to read.</param>
        /// <returns>The stored skill.</returns>
        protected Skill StoredSkill(SkillEnum skill)
        {
            return Repository.GetAllCharacters().Single().Skills[skill];
        }
    }

    /// <summary>
    ///     Increasing and decreasing a skill's mastery.
    /// </summary>
    public sealed class ChangeSkillMastery : SkillsEditFixture
    {
        /// <summary>
        ///     Left-clicking a skill at Normal mastery and saving raises it to Mastery in the repository.
        /// </summary>
        [Test]
        public async Task Should_Increase_Mastery_On_Left_Click()
        {
            // Act
            await Skills.IncreaseMastery("Athletics");
            await CaptureAsync("before-save");
            await Editor.Save();

            // Assert
            StoredSkill(SkillEnum.Athletics).Mastery.Should().Be(SkillMasteryEnum.Mastery);
        }

        /// <summary>
        ///     Right-clicking a skill at Expertise and saving lowers it to Mastery in the repository.
        /// </summary>
        [Test]
        public async Task Should_Decrease_Mastery_On_Right_Click()
        {
            // Act
            await Skills.DecreaseMastery("Stealth");
            await CaptureAsync("before-save");
            await Editor.Save();

            // Assert
            StoredSkill(SkillEnum.Stealth).Mastery.Should().Be(SkillMasteryEnum.Mastery);
        }
    }

    /// <summary>
    ///     Changing the ability a skill uses for its check.
    /// </summary>
    public sealed class ChangeGoverningAbility : SkillsEditFixture
    {
        /// <summary>
        ///     Picking another ability on a skill's default-ability chip and saving stores the new ability.
        /// </summary>
        [Test]
        public async Task Should_Save_Chosen_Ability()
        {
            // Act
            await Skills.SetAbility("Athletics", "DEX");
            await CaptureAsync("before-save");
            await Editor.Save();

            // Assert
            StoredSkill(SkillEnum.Athletics).Ability.Should().Be(AbilityEnum.Dexterity);
        }
    }
}
