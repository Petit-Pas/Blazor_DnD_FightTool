using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.TurnActions.EndTurn;
using DnDFightTool.Business.DnDActions.TurnActions.StartCombat;
using DnDFightTool.Business.DnDActions.TurnActions.SetCurrentFighter;
using DnDFightTool.Business.DnDActions.TurnActions.StartNextRound;
using DnDFightTool.Business.DnDActions.TurnActions.StartNextTurn;
using DnDFightTool.Business.DnDActions.TurnActions.StartTurn;
using DnDFightTool.Domain.Fight.Characters;
using DnDFightTool.Domain.Fight.TurnTracking;
using DomainTestsUtilities.Extensions;
using DomainTestsUtilities.Factories.Characters;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.TurnActions;

[TestFixture]
internal class StartNextTurnCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private ICombatTurnService _combatTurnService = null!;
    private StartNextTurnCommand _command = null!;
    private StartNextTurnCommandHandler _handler = null!;

    private FightingCharacter _fighter1 = null!;
    private FightingCharacter _fighter2 = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Implements<ISubCommandDispatcher>());
        _combatTurnService = A.Fake<ICombatTurnService>();
        _command = new StartNextTurnCommand();
        _handler = new StartNextTurnCommandHandler(_mediator, _combatTurnService);

        _fighter1 = CharacterFactory.BuildMonster(name: "Goblin").AsFighter();
        _fighter2 = CharacterFactory.BuildMonster(name: "Orc").AsFighter();
    }

    [TestFixture]
    internal class FirstTurnTests : StartNextTurnCommandHandlerTests
    {
        [SetUp]
        public new void SetUp()
        {
            A.CallTo(() => _combatTurnService.IsStarted).Returns(false);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns((FightingCharacter?)null);
            A.CallTo(() => _combatTurnService.GetNextFighter()).Returns(_fighter1);
            A.CallTo(() => _combatTurnService.IsLastTurnOfRound()).Returns(false);
        }

        [Test]
        public async Task Should_Return_Success()
        {
            // Act
            var response = await _handler.ExecuteAsync(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Dispatch_StartCombat_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<StartCombatCommand>._, A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Not_Dispatch_EndTurn_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<EndTurnCommand>._, A<ICommand>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Should_Dispatch_StartNextRound_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<StartNextRoundCommand>._, A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Dispatch_SetCurrentFighter_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<SetCurrentFighterCommand>.That.Matches(c => c.FighterId == _fighter1.Id),
                    A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Dispatch_StartTurn_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<StartTurnCommand>.That.Matches(c => c.FighterId == _fighter1.Id),
                    A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class SubsequentTurnTests : StartNextTurnCommandHandlerTests
    {
        [SetUp]
        public new void SetUp()
        {
            A.CallTo(() => _combatTurnService.IsStarted).Returns(true);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns(_fighter1);
            A.CallTo(() => _combatTurnService.GetNextFighter()).Returns(_fighter2);
            A.CallTo(() => _combatTurnService.IsLastTurnOfRound()).Returns(false);
        }

        [Test]
        public async Task Should_Dispatch_EndTurn_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<EndTurnCommand>.That.Matches(c => c.FighterId == _fighter1.Id),
                    A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Not_Dispatch_StartCombat_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<StartCombatCommand>._, A<ICommand>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Should_Not_Dispatch_StartNextRound_When_Not_LastTurn()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<StartNextRoundCommand>._, A<ICommand>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Should_Dispatch_SetCurrentFighter_With_NextFighter()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<SetCurrentFighterCommand>.That.Matches(c => c.FighterId == _fighter2.Id),
                    A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class RoundBoundaryTests : StartNextTurnCommandHandlerTests
    {
        [SetUp]
        public new void SetUp()
        {
            A.CallTo(() => _combatTurnService.IsStarted).Returns(true);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns(_fighter1);
            A.CallTo(() => _combatTurnService.GetNextFighter()).Returns(_fighter2);
            A.CallTo(() => _combatTurnService.IsLastTurnOfRound()).Returns(true);
        }

        [Test]
        public async Task Should_Dispatch_StartNextRound_When_LastTurn()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<StartNextRoundCommand>._, A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
