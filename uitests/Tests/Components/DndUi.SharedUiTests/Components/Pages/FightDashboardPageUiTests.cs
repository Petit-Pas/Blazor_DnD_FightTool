using DnDFightTool.Business.DnDActions.FightActions.AddToFight;
using DnDFightTool.Business.DnDActions.TurnActions.StartNextTurn;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.UiTests.Framework;
using DnDFightTool.UiTests.TestableComponents.Components.TestableDndUi.Shared.Components.Pages;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDFightTool.UiTests.Tests.Components.DndUi.SharedUiTests.Components.Pages;

/// <summary>
///     Independent fighter display, selection, combat progression and history capabilities.
/// </summary>
public static class FightDashboardPageUiTests
{
    /// <summary>
    ///     Prepares the common roster and dashboard object for each independent scenario.
    /// </summary>
    public abstract class DashboardFixture : IsolatedScenarioFixture
    {
        /// <summary>
        ///     Gets the mediator used for scenario-specific combat arrangement.
        /// </summary>
        protected IUndoableMediator Mediator { get; private set; } = null!;

        /// <summary>
        ///     Gets the dashboard object for the current scenario.
        /// </summary>
        protected TestableFightDashboardPage Dashboard { get; private set; } = null!;

        /// <summary>
        ///     Seeds two fighters before each test without starting combat or navigating.
        /// </summary>
        [SetUp]
        public async Task SetUpFight()
        {
            var repository = Services.GetRequiredService<ICharacterRepository>();
            Mediator = Services.GetRequiredService<IUndoableMediator>();
            var goblin = CharacterFactory.BuildMonster(name: "Goblin");
            var orc = CharacterFactory.BuildMonster(name: "Orc");
            goblin.HitPoints.MaxHps = 12;
            goblin.HitPoints.CurrentHps = 7;
            orc.HitPoints.MaxHps = 20;
            orc.HitPoints.CurrentHps = 13;
            repository.Save(goblin);
            repository.Save(orc);
            await Mediator.SendAsync(new AddToFightAtomicCommand(orc.Id, initiative: 10));
            await Mediator.SendAsync(new AddToFightAtomicCommand(goblin.Id, initiative: 15));
            Dashboard = new TestableFightDashboardPage(Page, BaseUrl, Mediator);
        }
    }

    /// <summary>
    ///     Reading the displayed fighter information without starting combat.
    /// </summary>
    public sealed class DisplayFighters : DashboardFixture
    {
        /// <summary>
        ///     Each fighter tile displays its own name, hit points and initiative.
        /// </summary>
        [Test]
        public async Task Should_Display_Fighter_Details()
        {
            // Act
            await Dashboard.GoToAsync();

            // Assert
            var goblinTile = Dashboard.FighterTile("Goblin");
            (await goblinTile.GetName()).Should().Contain("Goblin");
            (await goblinTile.GetCurrentHp()).Should().Be(7);
            (await goblinTile.GetMaxHp()).Should().Be(12);
            (await goblinTile.GetInitiative()).Should().Be(15);
            var orcTile = Dashboard.FighterTile("Orc");
            (await orcTile.GetName()).Should().Contain("Orc");
            (await orcTile.GetCurrentHp()).Should().Be(13);
            (await orcTile.GetMaxHp()).Should().Be(20);
            (await orcTile.GetInitiative()).Should().Be(10);
        }
    }

    /// <summary>
    ///     Selecting fighters in a combat that is already running.
    /// </summary>
    public sealed class SelectFighters : DashboardFixture
    {
        /// <summary>
        ///     Clicking another tile transfers the selected styling to that tile.
        /// </summary>
        [Test]
        public async Task Should_Select_Only_The_Chosen_Fighter()
        {
            // Arrange
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Dashboard.GoToAsync();
            await Dashboard.SelectFighter("Goblin");

            // Act
            var selected = await Dashboard.SelectFighter("Orc");

            // Assert
            (await selected.IsSelected()).Should().BeTrue();
            (await Dashboard.FighterTile("Goblin").IsSelected()).Should().BeFalse();
        }

        /// <summary>
        ///     A turn-service change transfers highlighting without clicking a tile.
        /// </summary>
        [Test]
        public async Task Should_Highlight_The_Fighter_Selected_By_The_Turn_Service()
        {
            await Dashboard.GoToAsync();
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Dashboard.FighterTile("Goblin").ExpectSelected(true);

            await Mediator.SendAsync(new StartNextTurnCommand());

            await Dashboard.FighterTile("Orc").ExpectSelected(true);
            await Dashboard.FighterTile("Goblin").ExpectSelected(false);
        }
    }

    /// <summary>
    ///     Starting combat from an arranged roster.
    /// </summary>
    public sealed class StartCombat : DashboardFixture
    {
        /// <summary>
        ///     Clicking Start Combat dispatches a command to the real mediator.
        /// </summary>
        [Test]
        public async Task Should_Dispatch_Start_Combat()
        {
            // Arrange
            await Dashboard.GoToAsync();
            var status = Dashboard.CombatStatus();
            var historyLength = Mediator.HistoryLength;

            // Act
            var command = await status.StartCombat();

            // Assert
            command.Should().BeOfType<StartNextTurnCommand>();
            Mediator.HistoryLength.Should().Be(historyLength + 1);
        }

        /// <summary>
        ///     Starting combat through the mediator refreshes the rendered panel.
        /// </summary>
        [Test]
        public async Task Should_Display_Service_Combat_State()
        {
            await Dashboard.GoToAsync();

            await Mediator.SendAsync(new StartNextTurnCommand());

            var status = Dashboard.CombatStatus();
            await status.WaitForRound(1);
            (await status.GetTurnText()).Should().Contain("Goblin");
        }
    }

    /// <summary>
    ///     Refreshing the displayed combat status through the Next Turn button.
    /// </summary>
    public sealed class AdvanceTurns : DashboardFixture
    {
        /// <summary>
        ///     Clicking Next Turn adds the requested command to mediator history.
        /// </summary>
        [Test]
        public async Task Should_Dispatch_Next_Turn()
        {
            // Arrange
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Dashboard.GoToAsync();
            var status = Dashboard.CombatStatus();
            var historyLength = Mediator.HistoryLength;

            // Act
            var command = await status.NextTurn();

            // Assert
            command.Should().BeOfType<StartNextTurnCommand>();
            Mediator.HistoryLength.Should().Be(historyLength + 1);
        }

        /// <summary>
        ///     Advancing through the mediator refreshes the round and fighter labels.
        /// </summary>
        [Test]
        public async Task Should_Render_An_Advanced_Service_Turn()
        {
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Dashboard.GoToAsync();

            await Mediator.SendAsync(new StartNextTurnCommand());

            var status = Dashboard.CombatStatus();
            await status.WaitForRound(2);
            (await status.GetTurnText()).Should().Contain("Goblin");
        }
    }

    /// <summary>
    ///     Operating Undo and observing the resulting panel and button state.
    /// </summary>
    public sealed class UndoCombatActions : DashboardFixture
    {
        /// <summary>
        ///     Clicking Undo moves the latest command from history to redo history.
        /// </summary>
        [Test]
        public async Task Should_Undo_The_Last_Mediator_Command()
        {
            // Arrange
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Dashboard.GoToAsync();
            var status = Dashboard.CombatStatus();
            var historyLength = Mediator.HistoryLength;
            var redoHistoryLength = Mediator.RedoHistoryLength;

            // Act
            await status.Undo();

            // Assert
            Mediator.HistoryLength.Should().Be(historyLength - 1);
            Mediator.RedoHistoryLength.Should().Be(redoHistoryLength + 1);
        }

        /// <summary>
        ///     Undoing through the mediator renders idle state and enables Redo.
        /// </summary>
        [Test]
        public async Task Should_Render_Undone_Service_State()
        {
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Dashboard.GoToAsync();

            await Mediator.UndoLastCommandAsync();

            await Dashboard.CombatStatus().WaitForRound(0);
            await Dashboard.CombatStatus().ExpectRedoEnabled();
        }
    }

    /// <summary>
    ///     Operating Redo from an independently arranged undone action.
    /// </summary>
    public sealed class RedoCombatActions : DashboardFixture
    {
        /// <summary>
        ///     Clicking Redo moves the last undone command back into history.
        /// </summary>
        [Test]
        public async Task Should_Redo_The_Last_Undone_Mediator_Command()
        {
            // Arrange
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Mediator.UndoLastCommandAsync();
            await Dashboard.GoToAsync();
            var status = Dashboard.CombatStatus();
            var historyLength = Mediator.HistoryLength;
            var redoHistoryLength = Mediator.RedoHistoryLength;

            // Act
            await status.Redo();

            // Assert
            Mediator.HistoryLength.Should().Be(historyLength + 1);
            Mediator.RedoHistoryLength.Should().Be(redoHistoryLength - 1);
        }

        /// <summary>
        ///     Redoing through the mediator restores the rendered combat state.
        /// </summary>
        [Test]
        public async Task Should_Render_Redone_Service_State()
        {
            await Mediator.SendAsync(new StartNextTurnCommand());
            await Mediator.UndoLastCommandAsync();
            await Dashboard.GoToAsync();

            await Mediator.RedoLastUndoneCommandAsync();

            await Dashboard.CombatStatus().WaitForRound(1);
            (await Dashboard.CombatStatus().GetTurnText()).Should().Contain("Goblin");
        }
    }

}
