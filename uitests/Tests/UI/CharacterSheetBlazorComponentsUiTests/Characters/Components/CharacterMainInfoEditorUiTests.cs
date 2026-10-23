using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Components;
using DnDFightTool.UiTests.TestableComponents.UI.TestableCharacterSheetBlazorComponents.Characters.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.UI.CharacterSheetBlazorComponentsUiTests.Characters.Components;

/// <summary>
///     Hit points, armor class and shield editing on the character editor's "Basic infos" tab. Name-level creation and
///     cancellation are already covered by <c>CharacterListEditorPageUiTests</c>.
/// </summary>
public static class CharacterMainInfoEditorUiTests
{
    /// <summary>
    ///     Rendering name, hit points, armor class and shield data seeded through the repository.
    /// </summary>
    public sealed class DisplayMainInfo : IsolatedScenarioFixture
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
        ///     The Basic infos tab renders the character's stored name, hit points, armor class, shield and the
        ///     resulting effective AC.
        /// </summary>
        [Test]
        public async Task Should_Display_Main_Infos()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter", withDefaults: true);
            fighter.HitPoints.MaxHps = 55;
            fighter.HitPoints.CurrentHps = 40;
            fighter.ArmorClass.BaseArmorClass = 15;
            fighter.ArmorClass.ShieldArmorClass = 2;
            fighter.ArmorClass.HasShieldEquipped = true;
            _repository.Save(fighter);
            await _characterList.GoToAsync();
            var editor = await _characterList.EditPlayer("Fighter");

            // Act
            var mainInfo = editor.MainInfo();

            // Assert
            (await mainInfo.GetName()).Should().Be("Fighter");
            (await mainInfo.GetMaxHps()).Should().Be(55);
            (await mainInfo.GetCurrentHps()).Should().Be(40);
            (await mainInfo.GetArmorClass()).Should().Be(15);
            (await mainInfo.GetShieldValue()).Should().Be(2);
            (await mainInfo.IsShieldEquipped()).Should().BeTrue();
            (await mainInfo.GetEffectiveAc()).Should().Be(17);
        }
    }

    /// <summary>
    ///     Opens the Basic infos tab of a seeded player whose shield is unequipped.
    /// </summary>
    public abstract class MainInfoEditFixture : IsolatedScenarioFixture
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
        ///     Gets the Basic infos form of the opened editor.
        /// </summary>
        protected TestableCharacterMainInfoEditor MainInfo { get; private set; } = null!;

        /// <summary>
        ///     Seeds the player and opens its editor.
        /// </summary>
        [SetUp]
        public async Task OpenMainInfo()
        {
            Repository = Services.GetRequiredService<ICharacterRepository>();
            var fighter = CharacterFactory.BuildPlayer(name: "Fighter", withDefaults: true);
            fighter.ArmorClass.HasShieldEquipped = false;
            Repository.Save(fighter);
            var characterList = new TestableCharacterListEditorPage(Page, BaseUrl);
            await characterList.GoToAsync();
            Editor = await characterList.EditPlayer("Fighter");
            MainInfo = Editor.MainInfo();
        }
    }

    /// <summary>
    ///     Saving edits to a character's hit points.
    /// </summary>
    public sealed class EditHitPoints : MainInfoEditFixture
    {
        /// <summary>
        ///     Saving updates both the max and current hit points in the repository.
        /// </summary>
        [Test]
        public async Task Should_Save_Hit_Points()
        {
            // Act
            await MainInfo.SetMaxHps(42);
            await MainInfo.SetCurrentHps(30);
            await Editor.Save();

            // Assert
            var stored = Repository.GetAllCharacters().Single();
            stored.HitPoints.MaxHps.Should().Be(42);
            stored.HitPoints.CurrentHps.Should().Be(30);
        }
    }

    /// <summary>
    ///     Saving edits to an existing character's name. Creation and cancellation of the name field are covered by
    ///     <c>CharacterListEditorPageUiTests</c>.
    /// </summary>
    public sealed class EditName : MainInfoEditFixture
    {
        /// <summary>
        ///     Saving an existing character under a new name updates the repository, replacing the original rather than
        ///     adding a second entry.
        /// </summary>
        [Test]
        public async Task Should_Save_Name()
        {
            // Act
            await MainInfo.SetName("Fighter Renamed");
            await Editor.Save();

            // Assert
            Repository.GetAllCharacters().Single().Name.Should().Be("Fighter Renamed");
        }
    }

    /// <summary>
    ///     Saving edits to a character's armor class and shield.
    /// </summary>
    public sealed class EditArmorClass : MainInfoEditFixture
    {
        /// <summary>
        ///     Saving updates the base armor class, the shield value, and equips the shield, starting from a seeded
        ///     character whose shield is unequipped.
        /// </summary>
        [Test]
        public async Task Should_Save_Armor_Class_And_Equip_Shield()
        {
            // Act
            await MainInfo.SetArmorClass(16);
            await MainInfo.SetShieldValue(3);
            await MainInfo.SetShieldEquipped(true);
            await Editor.Save();

            // Assert
            var stored = Repository.GetAllCharacters().Single();
            stored.ArmorClass.BaseArmorClass.Should().Be(16);
            stored.ArmorClass.ShieldArmorClass.Should().Be(3);
            stored.ArmorClass.HasShieldEquipped.Should().BeTrue();
        }
    }
}
