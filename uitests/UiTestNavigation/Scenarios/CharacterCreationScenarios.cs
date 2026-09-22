using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.UiTests.UiTestNavigation.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DnDFightTool.UiTests.UiTestNavigation.Scenarios;

/// <summary>
///     Scenarios proving character creation, duplication and deletion work through the typed page objects with no
///     Playwright locator in the scenario body.
/// </summary>
public sealed class CharacterCreationScenarios : IsolatedScenarioFixture
{
    /// <summary>
    ///     CREATE_PLAYER_VIA_UI: create a player through the UI (set name, save) and re-read the Players tab.
    /// </summary>
    [Test]
    public async Task Should_Create_A_Player_Through_The_Ui()
    {
        // Arrange
        var list = new TestCharacterListEditorPage(Page, BaseUrl);
        await list.GoToAsync();

        // Act
        var editor = await list.CreatePlayer();
        await editor.MainInfo().SetName("Aragorn");
        await CaptureAsync("player-name-entered");
        await editor.Save();

        // Assert
        (await list.PlayerNames()).Should().Contain("Aragorn");
    }

    /// <summary>
    ///     CREATE_MONSTER_VIA_UI: create a monster through the UI and re-read the Monsters tab.
    /// </summary>
    [Test]
    public async Task Should_Create_A_Monster_Through_The_Ui()
    {
        // Arrange
        var list = new TestCharacterListEditorPage(Page, BaseUrl);
        await list.GoToAsync();

        // Act
        var editor = await list.CreateMonster();
        await editor.MainInfo().SetName("Owlbear");
        await CaptureAsync("monster-name-entered");
        await editor.Save();

        // Assert
        (await list.MonsterNames()).Should().Contain("Owlbear");
    }

    /// <summary>
    ///     CANCEL_DISCARDS_NEW: start creating a player, then cancel — the character must not appear in the list. This also
    ///     exercises the editor's back-navigation path (see DW-007).
    /// </summary>
    [Test]
    public async Task Should_Discard_A_New_Character_On_Cancel()
    {
        // Arrange
        var list = new TestCharacterListEditorPage(Page, BaseUrl);
        await list.GoToAsync();

        // Act
        var editor = await list.CreatePlayer();
        await editor.MainInfo().SetName("Ghost");
        await editor.Cancel();

        // Assert
        (await list.PlayerNames()).Should().NotContain("Ghost");
    }

    /// <summary>
    ///     DUPLICATE_DELETE_CHAR: duplicate a seeded player (save adds a copy), then delete one copy and confirm the list.
    /// </summary>
    [Test]
    public async Task Should_Duplicate_Then_Delete_A_Player()
    {
        // Arrange
        var repository = Services.GetRequiredService<ICharacterRepository>();
        repository.Save(CharacterFactory.BuildPlayer(name: "Alpha"));
        repository.Save(CharacterFactory.BuildPlayer(name: "Beta"));

        var list = new TestCharacterListEditorPage(Page, BaseUrl);
        await list.GoToAsync();

        // Act: duplicate Alpha and save the copy.
        var editor = await list.DuplicatePlayer("Alpha");
        await editor.Save();
        await CaptureAsync("after-duplicate");
        (await list.PlayerNames()).Count(name => name == "Alpha").Should().Be(2);

        // Act: delete one Alpha.
        await list.DeletePlayer("Alpha");

        // Assert
        var names = await list.PlayerNames();
        names.Count(name => name == "Alpha").Should().Be(1);
        names.Should().Contain("Beta");
    }
}
