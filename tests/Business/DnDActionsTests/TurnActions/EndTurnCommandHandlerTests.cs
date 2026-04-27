using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.CloseBlock;
using DnDFightTool.Business.DnDActions.TurnActions.EndTurn;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.TurnActions;

[TestFixture]
internal class EndTurnCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private EndTurnCommand _command = null!;
    private EndTurnCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Implements<ISubCommandDispatcher>());
        _command = new EndTurnCommand(Guid.Empty);
        _handler = new EndTurnCommandHandler(_mediator);
    }

    [TestFixture]
    internal class ExecuteTests : EndTurnCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Act
            var response = await _handler.ExecuteAsync(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_Dispatch_CloseBlockCommand_As_SubCommand()
        {
            // Act
            await _handler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _mediator.SendAsSubCommandAsync(
                    A<CloseBlockCommand>._, A<ICommand>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}
