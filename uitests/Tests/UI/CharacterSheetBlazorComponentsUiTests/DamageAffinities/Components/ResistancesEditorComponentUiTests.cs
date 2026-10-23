using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.CharacterSheet.DamageAffinities;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.DamageAffinities.Components;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.DamageAffinities.Components;

/// <summary>
///     Damage-type affinity editing on the character editor's "Resistances" tab.
/// </summary>
public static class ResistancesEditorComponentUiTests
{
    /// <summary>
    ///     Rendering damage-affinity data seeded through the repository.
    /// </summary>
    public sealed class DisplayResistances : IsolatedScenarioFixture
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
        ///     Every damage type's resistance row renders its own seeded affinity, via the affinity button's accessible
        ///     name -- proving per-row correctness rather than just that one overridden row renders something, and that
        ///     overriding some damage types leaves the untouched ones at their default.
        /// </summary>
        [Test]
        public async Task Should_Display_Seeded_Affinity_For_Every_Damage_Type()
        {
            // Arrange
            var overrides = new Dictionary<DamageTypeEnum, DamageAffinityEnum>
            {
                [DamageTypeEnum.Fire] = DamageAffinityEnum.Resistant,
                [DamageTypeEnum.Cold] = DamageAffinityEnum.Immune,
                [DamageTypeEnum.Acid] = DamageAffinityEnum.Weak
            };
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter", withDefaults: true);
            foreach (var (damageType, affinity) in overrides)
            {
                fighter.DamageAffinities[damageType].Affinity = affinity;
            }

            _repository.Save(fighter);
            await _characterList.GoToAsync();
            var editor = await _characterList.EditPlayer("Fighter");

            // Act
            var resistances = await editor.OpenResistances();

            // Assert
            foreach (var damageType in DamageTypeEnumExtensions.All)
            {
                var expected = overrides.GetValueOrDefault(damageType, DamageAffinityEnum.Normal);
                (await resistances.GetAffinity(damageType.ToReadableString())).Should().Be(
                    expected.ToString(), $"{damageType} should render its seeded affinity");
            }
        }
    }

    /// <summary>
    ///     Increasing, decreasing, and canceling changes to a damage type's affinity.
    /// </summary>
    public sealed class ChangeAffinity : IsolatedScenarioFixture
    {
        private const DamageAffinityEnum SeededFireAffinity = DamageAffinityEnum.Normal;

        private ICharacterRepository _repository = null!;
        private TestableCharacterEditorPage _editor = null!;
        private TestableResistancesEditorComponent _resistances = null!;

        /// <summary>
        ///     Seeds a player whose Fire affinity is explicitly Normal and opens its resistances tab.
        /// </summary>
        [SetUp]
        public async Task OpenResistancesTab()
        {
            _repository = Services.GetRequiredService<ICharacterRepository>();
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter", withDefaults: true);
            fighter.DamageAffinities[DamageTypeEnum.Fire].Affinity = SeededFireAffinity;
            _repository.Save(fighter);
            var characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
            await characterList.GoToAsync();
            _editor = await characterList.EditPlayer("Fighter");
            _resistances = await _editor.OpenResistances();
        }

        /// <summary>
        ///     Left-clicking a damage type at Normal affinity and saving raises it to Resistant in the repository.
        /// </summary>
        [Test]
        public async Task Should_Increase_Affinity_On_Left_Click()
        {
            // Act
            await _resistances.IncreaseAffinity("Fire");
            await _editor.Save();

            // Assert
            StoredFireAffinity().Should().Be(DamageAffinityEnum.Resistant);
        }

        /// <summary>
        ///     Right-clicking a damage type at Normal affinity and saving lowers it to Weak in the repository.
        /// </summary>
        [Test]
        public async Task Should_Decrease_Affinity_On_Right_Click()
        {
            // Act
            await _resistances.DecreaseAffinity("Fire");
            await _editor.Save();

            // Assert
            StoredFireAffinity().Should().Be(DamageAffinityEnum.Weak);
        }

        /// <summary>
        ///     Canceling an affinity edit preserves the repository's original affinity.
        /// </summary>
        [Test]
        public async Task Should_Discard_Changes_On_Cancel()
        {
            // Act
            await _resistances.IncreaseAffinity("Fire");
            await _editor.Cancel();

            // Assert
            StoredFireAffinity().Should().Be(SeededFireAffinity);
        }

        private DamageAffinityEnum StoredFireAffinity()
        {
            return _repository.GetAllCharacters().Single().DamageAffinities[DamageTypeEnum.Fire].Affinity;
        }
    }
}
