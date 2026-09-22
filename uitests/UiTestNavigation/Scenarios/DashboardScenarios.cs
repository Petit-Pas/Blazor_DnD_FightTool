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
///     DASHBOARD_READ_AND_COMBAT: with two fighters arranged at a non-zero initiative (so the initiative modal never
///     opens), read the tiles and select one, then separately start combat and toggle undo/redo — all through the typed
///     dashboard objects.
/// </summary>
public sealed class DashboardScenarios : IsolatedScenarioFixture
{
    /// <summary>
    ///     Reads two seeded fighter tiles, starts combat (a selection prerequisite) and selects one.
    /// </summary>
    [Test]
    public async Task Should_Read_Tiles_And_Select_A_Fighter()
    {
        // Arrange: seed two fighters with a non-zero initiative so the dashboard renders without the initiative modal.
        var repository = Services.GetRequiredService<ICharacterRepository>();
        var mediator = Services.GetRequiredService<IUndoableMediator>();

        var goblin = CharacterFactory.BuildMonster(name: "Goblin");
        var orc = CharacterFactory.BuildMonster(name: "Orc");
        repository.Save(goblin);
        repository.Save(orc);
        await mediator.SendAsync(new AddToFightAtomicCommand(goblin.Id, initiative: 15));
        await mediator.SendAsync(new AddToFightAtomicCommand(orc.Id, initiative: 15));

        var dashboard = new TestFightDashboardPage(Page, BaseUrl, mediator);
        await dashboard.GoToAsync();
        await CaptureAsync("dashboard-two-fighters");

        // Assert: both tiles render with readable name, HP and initiative.
        var goblinTile = dashboard.FighterTile("Goblin");
        (await goblinTile.GetName()).Should().Contain("Goblin");
        (await goblinTile.GetCurrentHp()).Should().Be(10);
        (await goblinTile.GetMaxHp()).Should().Be(10);
        (await goblinTile.GetInitiative()).Should().BeGreaterThanOrEqualTo(0);
        (await dashboard.FighterTile("Orc").GetName()).Should().Contain("Orc");

        // Act + Assert: selection only takes effect once combat has started, so start it first.
        await dashboard.CombatStatus().StartCombat();
        var selected = await dashboard.SelectFighter("Goblin");
        (await selected.IsSelected()).Should().BeTrue();
    }

    /// <summary>
    ///     Starts combat, then undoes and redoes it, checking only the combat-status state.
    /// </summary>
    [Test]
    public async Task Should_Undo_And_Redo_Combat_Start()
    {
        // Arrange: seed two fighters with a non-zero initiative so the dashboard renders without the initiative modal.
        var repository = Services.GetRequiredService<ICharacterRepository>();
        var mediator = Services.GetRequiredService<IUndoableMediator>();

        var goblin = CharacterFactory.BuildMonster(name: "Goblin");
        var orc = CharacterFactory.BuildMonster(name: "Orc");
        repository.Save(goblin);
        repository.Save(orc);
        await mediator.SendAsync(new AddToFightAtomicCommand(goblin.Id, initiative: 15));
        await mediator.SendAsync(new AddToFightAtomicCommand(orc.Id, initiative: 15));

        var dashboard = new TestFightDashboardPage(Page, BaseUrl, mediator);
        await dashboard.GoToAsync();

        // Act + Assert: start combat.
        var status = dashboard.CombatStatus();
        await status.StartCombat();
        await CaptureAsync("combat-started");
        (await status.GetRound()).Should().Be(1);

        // Act + Assert: undo and redo the combat start.
        await status.Undo();
        (await status.IsCombatStarted()).Should().BeFalse();
        await status.Redo();
        (await status.IsCombatStarted()).Should().BeTrue();
        (await status.GetRound()).Should().Be(1);
    }
}
