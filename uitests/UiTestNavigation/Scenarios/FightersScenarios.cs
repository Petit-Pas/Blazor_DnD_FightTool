using DnDFightTool.Business.DnDActions.FightActions.AddToFight;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.UiTests.UiTestNavigation.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDFightTool.UiTests.UiTestNavigation.Scenarios;

/// <summary>
///     FIGHTERS_ADD_SEARCH: search the available monsters, add one to the fight, then remove it — all through the typed
///     fighters page object. Adding a first-of-kind fighter opens the initiative modal (story 6); this story side-steps it
///     by arranging one same-kind fighter in the fight up front, so the UI add inherits its initiative and no modal opens.
/// </summary>
public sealed class FightersScenarios : IsolatedScenarioFixture
{
    /// <summary>
    ///     Searches, adds and removes a monster through the UI without opening the initiative modal.
    /// </summary>
    [Test]
    public async Task Should_Search_Add_And_Remove_A_Monster()
    {
        // Arrange: seed the templates, and arrange one Goblin already in the fight so a UI add inherits its initiative.
        var repository = Services.GetRequiredService<ICharacterRepository>();
        var mediator = Services.GetRequiredService<IUndoableMediator>();
        repository.Save(CharacterFactory.BuildPlayer(name: "Hero"));
        var goblin = CharacterFactory.BuildMonster(name: "Goblin");
        repository.Save(goblin);
        await mediator.SendAsync(new AddToFightAtomicCommand(goblin.Id, initiative: 15));

        var fighters = new TestFightersPage(Page, BaseUrl);
        await fighters.GoToAsync();

        // Act + Assert: the monster is available before filtering.
        (await fighters.AvailableMonsterNames()).Should().Contain(name => name.Contains("Goblin"));

        // Act + Assert: a matching search keeps it, a non-matching search hides it.
        await fighters.SearchMonsters("Gob");
        (await fighters.AvailableMonsterNames()).Should().ContainSingle().Which.Should().Contain("Goblin");
        await fighters.SearchMonsters("no-such-monster");
        await CaptureAsync("monster-search-no-match");
        (await fighters.AvailableMonsterNames()).Should().BeEmpty();
        await fighters.SearchMonsters(string.Empty);

        // Act + Assert: add a second Goblin through the UI (inherits initiative, no modal).
        await fighters.AddMonster("Goblin");
        await CaptureAsync("goblin-added");
        (await fighters.InFightNames()).Should().Contain(name => name.Contains("2") && name.Contains("Goblin"));

        // Act + Assert: remove one Goblin through the UI.
        await fighters.RemoveFromFight("Goblin");
        (await fighters.InFightNames()).Should().Contain(name => name.Contains("Goblin"));
        (await fighters.InFightNames()).Should().NotContain(name => name.Contains("2") && name.Contains("Goblin"));
    }
}
