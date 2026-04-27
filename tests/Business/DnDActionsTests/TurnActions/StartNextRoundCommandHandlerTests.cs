using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Business.DnDActions.TurnActions.StartNextRound;
using DnDFightTool.Domain.Fight.TurnTracking;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.TurnActions;

[TestFixture]
internal class StartNextRoundCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private ICombatTurnService _combatTurnService = null!;
    private StartNextRoundCommand _command = null!;
    private StartNextRoundCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Implements<ISubCommandDispatcher>());
        _combatTurnService = A.Fake<ICombatTurnService>();
        _command = new StartNextRoundCommand();
        _handler = new StartNextRoundCommandHandler(_mediator, _combatTurnService);
    }

    [TestFixture]
    internal class ExecuteTests : StartNextRoundCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Arrange
            A.CallTo(() => _combatTurnService.CurrentRound).Returns(1);

            // Act
            var response = await _handler.ExecuteAsync(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Store_PreviousRound()
        {
            // Arrange
            A.CallTo(() => _combatTurnService.CurrentRound).Returns(2);

            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            _command.PreviousRound.Should().Be(2);
        }

        [Test]
        public async Task Should_Increment_Round()
        {
            // Arrange
            A.CallTo(() => _combatTurnService.CurrentRound).Returns(2);

            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _combatTurnService.SetCurrentRound(3))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Dispatch_WriteLogCommand_As_SubCommand()
        {
            // Arrange
            A.CallTo(() => _combatTurnService.CurrentRound).Returns(0);

            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<WriteLogCommand>._, A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class UndoTests : StartNextRoundCommandHandlerTests
    {
        [Test]
        public async Task Should_Restore_PreviousRound()
        {
            // Arrange
            _command.PreviousRound = 3;

            // Act
            await _handler.UndoAsync(_command);

            // Assert
            A.CallTo(() => _combatTurnService.SetCurrentRound(3))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    internal class RedoTests : StartNextRoundCommandHandlerTests
    {
        [Test]
        public async Task Should_Increment_Round_From_PreviousRound()
        {
            // Arrange
            _command.PreviousRound = 2;

            // Act
            await _handler.RedoAsync(_command);

            // Assert
            A.CallTo(() => _combatTurnService.SetCurrentRound(3))
                .MustHaveHappenedOnceExactly();
        }
    }
}
