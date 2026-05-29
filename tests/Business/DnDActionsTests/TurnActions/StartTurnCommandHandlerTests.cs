using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.OpenBlock;
using DnDFightTool.Business.DnDActions.TurnActions.StartTurn;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
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
internal class StartTurnCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IFightContext _fightContext = null!;
    private StartTurnCommand _command = null!;
    private StartTurnCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict().Implements<ISubCommandDispatcher>());
        _fightContext = A.Fake<IFightContext>(options => options.Strict());
        _command = new StartTurnCommand(Guid.Empty);
        _handler = new StartTurnCommandHandler(_mediator, _fightContext);

        A.CallTo(() => _mediator.SendAsSubCommandAsync(A<OpenBlockCommand>._, A<ICommand>._)).Returns(CommandResponse.Success());
    }

    [TestFixture]
    internal class ExecuteTests : StartTurnCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Arrange
            var fighter = CharacterFactory.BuildMonster(name: "Goblin").AsFighter();
            _command.FighterId = fighter.Id;
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);

            // Act
            var response = await _handler.ExecuteAsync(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Dispatch_OpenBlockCommand_With_FighterName()
        {
            // Arrange
            var fighter = CharacterFactory.BuildMonster(name: "Goblin").AsFighter();
            _command.FighterId = fighter.Id;
            A.CallTo(() => _fightContext[fighter.Id]).Returns(fighter);

            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<OpenBlockCommand>.That.Matches(c => c.Name == "Goblin's turn"),
                    A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Should_Throw_When_FighterNotFound()
        {
            // Arrange
            _command.FighterId = Guid.NewGuid();
            A.CallTo(() => _fightContext[_command.FighterId]).Returns(null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.ExecuteAsync(_command));
        }
    }
}
