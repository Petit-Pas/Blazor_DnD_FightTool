using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Business.DnDActions.TurnActions.SetCurrentFighter;
using DnDFightTool.Domain.Fight;
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

namespace DnDActionsTests.FightActions;

[TestFixture]
internal class RemoveFromFightCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;
    private ICombatTurnService _combatTurnService = null!;

    private RemoveFromFightCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());
        _combatTurnService = A.Fake<ICombatTurnService>(options => options.Strict());

        _handler = new RemoveFromFightCommandHandler(_mediator, _fightContext, _combatTurnService);

        // Default: allow all sub-commands the orchestrator always sends
        A.CallTo(() => _mediator.SendAsSubCommandAsync(A<RemoveFromFightAtomicCommand>._, A<RemoveFromFightCommand>._))
            .Returns(Task.FromResult<ICommandResponse<NoResponse>>(CommandResponse.Success()));
        A.CallTo(() => _mediator.SendAsSubCommandAsync(A<WriteLogCommand>._, A<RemoveFromFightCommand>._))
            .Returns(Task.FromResult<ICommandResponse<NoResponse>>(CommandResponse.Success()));
    }

    [TestFixture]
    internal class ExecuteTests : RemoveFromFightCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success_When_Fighter_Exists()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Aragorn").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns((IFightingCharacter?)null);

            var command = new RemoveFromFightCommand(fighter.Id);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Send_EvictFighterCommand()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Legolas").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns((IFightingCharacter?)null);

            var command = new RemoveFromFightCommand(fighter.Id);

            // Act
            await _handler.ExecuteAsync(command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<RemoveFromFightAtomicCommand>.That.Matches(c => c.FighterId == fighter.Id),
                A<RemoveFromFightCommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Send_SetCurrentFighterCommand_Null_When_Removing_ActiveFighter()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Gandalf").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns(fighter);
            A.CallTo(() => _mediator.SendAsSubCommandAsync(A<SetCurrentFighterCommand>._, A<RemoveFromFightCommand>._))
                .Returns(Task.FromResult<ICommandResponse<NoResponse>>(CommandResponse.Success()));

            var command = new RemoveFromFightCommand(fighter.Id);

            // Act
            await _handler.ExecuteAsync(command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<SetCurrentFighterCommand>.That.Matches(c => c.FighterId == null),
                A<RemoveFromFightCommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Not_Send_SetCurrentFighterCommand_When_NonActiveFighter()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Aragorn").AsFighter();
            var otherFighter = CharacterFactory.BuildPlayer(name: "Legolas").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns(otherFighter);

            var command = new RemoveFromFightCommand(fighter.Id);

            // Act
            await _handler.ExecuteAsync(command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<SetCurrentFighterCommand>._,
                A<RemoveFromFightCommand>._))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Should_Return_Failed_When_Fighter_Not_Found()
        {
            // Arrange
            var missingId = Guid.NewGuid();
            A.CallTo(() => _fightContext[missingId]).Returns((IFightingCharacter?)null);

            var command = new RemoveFromFightCommand(missingId);

            // Act
            var response = await _handler.ExecuteAsync(command);

            // Assert
            response.Status.Should().Be(RequestStatus.Failed);
        }

        [Test]
        public async Task Should_Send_WriteLogCommand_SubCommand()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Gimli").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns((IFightingCharacter?)null);

            var command = new RemoveFromFightCommand(fighter.Id);

            // Act
            await _handler.ExecuteAsync(command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<WriteLogCommand>.That.Matches(x => x.Content.Contains("left the fight")),
                A<RemoveFromFightCommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class RedoTests : RemoveFromFightCommandHandlerTests
    {
        [Test]
        public async Task Should_Send_EvictFighterCommand_Again()
        {
            // Arrange
            var fighter = CharacterFactory.BuildPlayer(name: "Aragorn").AsFighter();
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);
            A.CallTo(() => _combatTurnService.CurrentTurnFighter).Returns((IFightingCharacter?)null);

            var command = new RemoveFromFightCommand(fighter.Id);

            // Act
            await _handler.RedoAsync(command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                A<RemoveFromFightAtomicCommand>.That.Matches(c => c.FighterId == fighter.Id),
                A<RemoveFromFightCommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
