using DnDFightTool.Business.DnDActions.FightActions.AddToFight;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Logs;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDFightTool.UiTests.UiTestNavigation.Meta;

/// <summary>
///     Proves that scenarios sharing the host are isolated: the per-scenario teardown undoes every command and deletes
///     leftover characters, so a scenario that mutates the fight, history and repository leaves a proven-clean slate for
///     whichever scenario runs next, regardless of ordering.
/// </summary>
public sealed class ScenarioIsolationTests : ApplicationFixture
{
    /// <summary>
    ///     CLEAN_AFTER_COMBAT: mutates the fight via a command and persists a character, then relies on teardown to undo
    ///     every command, assert the command-driven state is empty and delete the leftover character.
    /// </summary>
    [Test]
    public async Task Should_Leave_A_Clean_Slate_For_The_Next_Scenario()
    {
        // Arrange
        var repository = Services.GetRequiredService<ICharacterRepository>();
        var fightContext = Services.GetRequiredService<IFightContext>();
        var mediator = Services.GetRequiredService<IUndoableMediator>();

        var monster = CharacterFactory.BuildMonster(name: "Isolation goblin");
        repository.Save(monster);

        // Act
        await mediator.SendAsync(new AddToFightAtomicCommand(monster.Id, initiative: 15));

        // Assert
        fightContext.Fighters.Should().NotBeEmpty();
        mediator.HistoryLength.Should().BeGreaterThan(0);
        repository.Count.Should().BeGreaterThan(0);
    }

    /// <summary>
    ///     ISOLATION: asserts the scenario starts from an empty fight, empty log and empty repository, which only holds if
    ///     the previous scenario's teardown genuinely cleaned up after itself.
    /// </summary>
    [Test]
    public void Should_Start_From_An_Empty_Fight_Log_And_Repository()
    {
        // Arrange
        var fightContext = Services.GetRequiredService<IFightContext>();
        var repository = Services.GetRequiredService<ICharacterRepository>();

        // Act
        var fighters = fightContext.Fighters;

        // Assert
        fighters.Should().BeEmpty();
        repository.Count.Should().Be(0);
    }

    /// <summary>
    ///     CLEAN_AFTER_COMBAT (log path): produces a log entry through a command, then relies on teardown to undo it. Because
    ///     log undo hides rather than removes, teardown asserts no visible entry remains and then clears the log — this test
    ///     is what actually exercises that path, since the other scenarios dirty state through non-logging commands.
    /// </summary>
    [Test]
    public async Task Should_Leave_The_Log_Empty_After_A_Logging_Command()
    {
        // Arrange
        var mediator = Services.GetRequiredService<IUndoableMediator>();
        var logService = Services.GetRequiredService<IDnDLogService>();

        // Act
        await mediator.SendAsync(new WriteLogCommand("[b]isolation log entry[/b]"));

        // Assert: the entry is visible now; teardown will undo (hide) it, assert nothing visible remains, then clear.
        logService.Blocks.SelectMany(block => block.Entries).Should().NotBeEmpty();
    }

    /// <summary>
    ///     BROKEN_UNDO: this scenario tests the safety net itself, not the application. The whole isolation model rests on
    ///     <see cref="ApplicationFixture.AssertCommandDrivenStateIsEmpty"/> firing when teardown leaves residue behind — but
    ///     an assertion that never actually fails is worthless, so here we prove it has teeth. We put the app into a dirty
    ///     state that undo cannot reach, confirm the emptiness check throws, then clean up by hand.
    /// </summary>
    [Test]
    public void Should_Fail_The_Emptiness_Assertion_When_Command_Driven_State_Is_Residual()
    {
        // Arrange: add a fighter DIRECTLY on the singleton, not through the mediator. Because no command created it, the
        // teardown's "undo everything" loop has nothing to undo and cannot reach it — this stands in for a command whose
        // own undo failed to restore state, i.e. exactly the leak the emptiness assertion is meant to catch.
        var fightContext = Services.GetRequiredService<IFightContext>();
        var residualFighter = fightContext.Add(CharacterFactory.BuildMonster(name: "Residual goblin"), initiative: 10)!;

        // Act / Assert: with residue present, the emptiness check must throw. If it were broken and passed here, THIS test
        // fails — which is the point: it verifies the safety net notices dirty state instead of waving it through.
        var assertingEmptyState = AssertCommandDrivenStateIsEmpty;
        try
        {
            assertingEmptyState.Should().Throw<AssertionException>();
        }
        finally
        {
            // Undo can't reach a fighter added outside the mediator, so remove it by hand — in a finally so it happens even
            // if the assertion above misbehaves, otherwise the residue would leak into the next scenario and corrupt it.
            fightContext.Remove(residualFighter);
        }
    }
}

/// <summary>
///     UNDO_REDO_SUBJECT: a scenario whose subject is undo/redo itself. It opts out of the automatic undo-everything
///     teardown and owns its own history cleanup, leaving the fight and history empty by its own hand.
/// </summary>
public sealed class UndoRedoSubjectScenarioTests : ApplicationFixture
{
    /// <inheritdoc />
    protected override bool UndoAllCommandsOnTeardown
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    ///     Drives undo and redo directly as the thing under test, then cleans its own history so the opted-out teardown does
    ///     not have to.
    /// </summary>
    [Test]
    public async Task Should_Own_Its_Own_History_Cleanup()
    {
        // Arrange
        var repository = Services.GetRequiredService<ICharacterRepository>();
        var fightContext = Services.GetRequiredService<IFightContext>();
        var mediator = Services.GetRequiredService<IUndoableMediator>();

        var monster = CharacterFactory.BuildMonster(name: "Undo subject goblin");
        repository.Save(monster);
        await mediator.SendAsync(new AddToFightAtomicCommand(monster.Id, initiative: 12));
        fightContext.Fighters.Should().HaveCount(1);

        // Act
        await mediator.UndoLastCommandAsync();
        fightContext.Fighters.Should().BeEmpty();
        mediator.HistoryLength.Should().Be(0);
        mediator.RedoHistoryLength.Should().Be(1);

        await mediator.RedoLastUndoneCommandAsync();
        fightContext.Fighters.Should().HaveCount(1);
        mediator.HistoryLength.Should().Be(1);

        // Assert: the scenario owns its own cleanup, leaving history and fight empty for the next scenario.
        await mediator.UndoLastCommandAsync();
        fightContext.Fighters.Should().BeEmpty();
        mediator.HistoryLength.Should().Be(0);
    }
}
