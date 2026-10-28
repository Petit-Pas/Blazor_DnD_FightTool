using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.Framework.Extensions;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.Tests.Components.DndUi.SharedUiTests.Components.Pages;

/// <summary>
///     Independent creation, editing and deletion capabilities of the character library.
/// </summary>
public static class CharacterListEditorPageUiTests
{
    /// <summary>
    ///     Resolves the real repository and builds the list page object before each independent scenario.
    /// </summary>
    public abstract class CharacterLibraryFixture : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Gets the real character repository.
        /// </summary>
        protected ICharacterRepository Repository { get; private set; } = null!;

        /// <summary>
        ///     Gets the character list page object for the current scenario.
        /// </summary>
        protected TestableCharacterListEditorPage CharacterList { get; private set; } = null!;

        /// <summary>
        ///     Resolves the repository and constructs the page object without navigating.
        /// </summary>
        [SetUp]
        public void SetUpLibraryObjects()
        {
            Repository = Services.GetRequiredService<ICharacterRepository>();
            CharacterList = new TestableCharacterListEditorPage(Page, BaseUrl);
        }
    }

    /// <summary>
    ///     Rendering repository state without first creating it through the UI.
    /// </summary>
    public sealed class DisplayCharacters : CharacterLibraryFixture
    {
        /// <summary>
        ///     Seeds separate player and monster records before navigation.
        /// </summary>
        [SetUp]
        public void SetUpLibrary()
        {
            Repository.Save(CharacterFactory.BuildPlayer(name: "Aragorn"));
            Repository.Save(CharacterFactory.BuildMonster(name: "Owlbear"));
        }

        /// <summary>
        ///     The player list renders repository players.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_Players()
        {
            await CharacterList.GoToAsync();

            (await CharacterList.PlayerNames()).Should().Equal("Aragorn");
        }

        /// <summary>
        ///     The monster list renders repository monsters.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_Monsters()
        {
            await CharacterList.GoToAsync();

            (await CharacterList.MonsterNames()).Should().Equal("Owlbear");
        }

        /// <summary>
        ///     The player editor renders the repository name on entry.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_Player_In_Editor()
        {
            await CharacterList.GoToAsync();
            var editor = await CharacterList.EditPlayer("Aragorn");

            (await editor.MainInfo().GetName()).Should().Be("Aragorn");
        }

        /// <summary>
        ///     The monster editor renders the repository name on entry.
        /// </summary>
        [Test]
        public async Task Should_Display_Repository_Monster_In_Editor()
        {
            await CharacterList.GoToAsync();
            var editor = await CharacterList.EditMonster("Owlbear");

            (await editor.MainInfo().GetName()).Should().Be("Owlbear");
        }
    }

    /// <summary>
    ///     Creating characters from scratch or by duplication, including cancellation.
    /// </summary>
    public sealed class CreateCharacters : CharacterLibraryFixture
    {
        /// <summary>
        ///     Saving a new player writes it to the real repository.
        /// </summary>
        [Test]
        public async Task Should_Create_A_Player_Through_The_Ui()
        {
            // Arrange
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.CreatePlayer();
            await editor.MainInfo().SetName("Aragorn");
            await CaptureAsync("player-name-entered");
            await editor.Save();

            // Assert
            Repository.GetAllCharacters()
                .Should().ContainSingle(character => character.Name == "Aragorn" && character.Type == CharacterType.Player);
        }

        /// <summary>
        ///     Saving a new monster writes it to the real repository.
        /// </summary>
        [Test]
        public async Task Should_Create_A_Monster_Through_The_Ui()
        {
            // Arrange
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.CreateMonster();
            await editor.MainInfo().SetName("Owlbear");
            await CaptureAsync("monster-name-entered");
            await editor.Save();
            // Save returns to the list defaulted to Players; switch back so the automatic final screenshot shows the monster.
            await CharacterList.GoToMonstersTab();

            // Assert
            Repository.GetAllCharacters()
                .Should().ContainSingle(character => character.Name == "Owlbear" && character.Type == CharacterType.Monster);
        }

        /// <summary>
        ///     CANCEL_DISCARDS_NEW: start creating a player, then cancel — the character must not appear in the list. This
        ///     also exercises the editor's back-navigation path (see DW-016).
        /// </summary>
        [Test]
        public async Task Should_Discard_A_New_Character_On_Cancel()
        {
            // Arrange
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.CreatePlayer();
            await editor.MainInfo().SetName("Ghost");
            await CaptureAsync("before-cancel");
            await editor.Cancel();

            // Assert
            Repository.GetAllCharacters().Should().BeEmpty();
        }

        /// <summary>
        ///     Saving a duplicate creates another player without replacing the original.
        /// </summary>
        [Test]
        public async Task Should_Create_A_Player_By_Duplication()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildPlayer(name: "Alpha"));
            Repository.Save(CharacterFactory.BuildPlayer(name: "Beta"));
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.DuplicatePlayer("Alpha");
            await CaptureAsync("before-save");
            await editor.Save();

            // Assert
            Repository.GetAllCharacters().Where(character => character.Type == CharacterType.Player)
                .Select(character => character.Name).Should().BeEquivalentTo("Alpha", "Alpha", "Beta");
        }

        /// <summary>
        ///     Saving a duplicate creates another monster without replacing the original.
        /// </summary>
        [Test]
        public async Task Should_Create_A_Monster_By_Duplication()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildMonster(name: "Alpha"));
            Repository.Save(CharacterFactory.BuildMonster(name: "Beta"));
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.DuplicateMonster("Alpha");
            await CaptureAsync("before-save");
            await editor.Save();
            // Save returns to the list defaulted to Players; switch back so the automatic final screenshot shows the monsters.
            await CharacterList.GoToMonstersTab();

            // Assert
            Repository.GetAllCharacters().Where(character => character.Type == CharacterType.Monster)
                .Select(character => character.Name).Should().BeEquivalentTo("Alpha", "Alpha", "Beta");
        }

        /// <summary>
        ///     Canceling a duplicate leaves the original library unchanged.
        /// </summary>
        [Test]
        public async Task Should_Discard_A_Duplicate_On_Cancel()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildPlayer(name: "Original"));
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.DuplicatePlayer("Original");
            await editor.MainInfo().SetName("Discarded");
            await CaptureAsync("before-cancel");
            await editor.Cancel();

            // Assert
            Repository.GetAllCharacters()
                .Select(character => character.Name).Should().Equal("Original");
        }
    }

    /// <summary>
    ///     Deleting the selected row without affecting other displayed characters.
    /// </summary>
    public sealed class DeleteCharacters : CharacterLibraryFixture
    {
        /// <summary>
        ///     Clicking Delete removes only the selected player from the repository.
        /// </summary>
        [Test]
        public async Task Should_Delete_Only_The_Selected_Player()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildPlayer(name: "Target"));
            Repository.Save(CharacterFactory.BuildPlayer(name: "Unrelated"));
            await CharacterList.GoToAsync();

            // Act
            await CharacterList.DeletePlayer("Target");

            // Assert
            await Repository.Eventually(repository => repository.GetAllCharacters()
                .Select(character => character.Name).Should().Equal("Unrelated"));
        }

        /// <summary>
        ///     Clicking Delete removes only the selected monster from the repository.
        /// </summary>
        [Test]
        public async Task Should_Delete_Only_The_Selected_Monster()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildMonster(name: "Target"));
            Repository.Save(CharacterFactory.BuildMonster(name: "Unrelated"));
            await CharacterList.GoToAsync();

            // Act
            await CharacterList.DeleteMonster("Target");

            // Assert
            await Repository.Eventually(repository => repository.GetAllCharacters()
                .Select(character => character.Name).Should().Equal("Unrelated"));
        }
    }

    /// <summary>
    ///     Saving or canceling edits to existing characters.
    /// </summary>
    public sealed class EditCharacters : CharacterLibraryFixture
    {
        /// <summary>
        ///     Saving an edit replaces the selected player's name without creating another entry.
        /// </summary>
        [Test]
        public async Task Should_Save_Changes_To_An_Existing_Player()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildPlayer(name: "Original"));
            Repository.Save(CharacterFactory.BuildPlayer(name: "Unrelated"));
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.EditPlayer("Original");
            await editor.MainInfo().SetName("Renamed");
            await CaptureAsync("before-save");
            await editor.Save();

            // Assert
            Repository.GetAllCharacters().Select(character => character.Name)
                .Should().BeEquivalentTo("Renamed", "Unrelated");
        }

        /// <summary>
        ///     Saving an edit replaces the selected monster's name without creating another entry.
        /// </summary>
        [Test]
        public async Task Should_Save_Changes_To_An_Existing_Monster()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildMonster(name: "Original"));
            Repository.Save(CharacterFactory.BuildMonster(name: "Unrelated"));
            await CharacterList.GoToAsync();

            // Act
            var editor = await CharacterList.EditMonster("Original");
            await editor.MainInfo().SetName("Renamed");
            await CaptureAsync("before-save");
            await editor.Save();
            // Save returns to the list defaulted to Players; switch back so the automatic final screenshot shows the monster.
            await CharacterList.GoToMonstersTab();

            // Assert
            Repository.GetAllCharacters().Select(character => character.Name)
                .Should().BeEquivalentTo("Renamed", "Unrelated");
        }

        /// <summary>
        ///     Canceling an edit preserves the repository name.
        /// </summary>
        [Test]
        public async Task Should_Discard_Changes_On_Cancel()
        {
            // Arrange
            Repository.Save(CharacterFactory.BuildPlayer(name: "Original"));
            await CharacterList.GoToAsync();
            var editor = await CharacterList.EditPlayer("Original");

            // Act
            await editor.MainInfo().SetName("Discarded");
            await CaptureAsync("before-cancel");
            await editor.Cancel();

            // Assert
            Repository.GetAllCharacters()
                .Select(character => character.Name).Should().Equal("Original");
        }
    }

}
