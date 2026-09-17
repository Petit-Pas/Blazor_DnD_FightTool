using DnDFightTool.Business.DnDActions.FightActions.AddToFight;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DomainTestsUtilities.Factories.Characters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UndoableMediator.Mediators;

namespace DnDFightTool.UiTests.UiTestNavigation.Meta;

/// <summary>
///     AUTOMATIC_FINAL (error path) / AC4: proves the automatic <c>final</c> capture is best-effort. This fixture forces
///     every capture to throw, so the automatic capture in <c>CleanUpScenarioAsync</c> fails. The scenario arranges real
///     command-driven and persisted state, so it passes only if the best-effort catch lets the isolation teardown still
///     undo the command, assert the state empty and delete the character — a screenshot failure must not corrupt the next
///     scenario. Without the catch, the throw escapes before any cleanup runs and this test fails in teardown.
/// </summary>
public sealed class FinalCaptureFailureTests : IsolatedScenarioFixture
{
    /// <summary>
    ///     Forces every capture — including the automatic <c>final</c> capture in teardown — to throw.
    /// </summary>
    /// <param name="name">Unused; the override throws before writing anything.</param>
    /// <returns>Never returns; always throws.</returns>
    protected override Task<string> CaptureAsync(string name)
    {
        throw new IOException("Simulated screenshot failure to prove the automatic final capture is best-effort.");
    }

    /// <summary>
    ///     Dirties the fight, history and repository, then leaves cleanup to the teardown. The teardown's automatic
    ///     screenshot throws first; the scenario passes only if the best-effort catch lets the undo loop, emptiness
    ///     assertions and character deletion still run to completion.
    /// </summary>
    [Test]
    public async Task Should_Complete_Isolation_Cleanup_When_The_Automatic_Final_Capture_Throws()
    {
        // Arrange
        var repository = Services.GetRequiredService<ICharacterRepository>();
        var mediator = Services.GetRequiredService<IUndoableMediator>();

        var monster = CharacterFactory.BuildMonster(name: "Best-effort goblin");
        repository.Save(monster);

        // Act
        await mediator.SendAsync(new AddToFightAtomicCommand(monster.Id, initiative: 12));

        // Assert: state is genuinely dirty now, so the teardown has real work to undo despite the screenshot failure.
        mediator.HistoryLength.Should().BeGreaterThan(0);
        repository.Count.Should().BeGreaterThan(0);
    }
}
