using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.CloseBlock;
using DnDFightTool.Domain.Logs;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.LogActions;

[TestFixture]
internal class CloseBlockCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IDnDLogService _logService = null!;
    private CloseBlockCommand _command = null!;
    private CloseBlockCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _logService = A.Fake<IDnDLogService>();
        _command = new CloseBlockCommand();
        _commandHandler = new CloseBlockCommandHandler(_mediator, _logService);
    }

    [TestFixture]
    private class ExecuteTests : CloseBlockCommandHandlerTests
    {
        [Test]
        public async Task Should_Return_Success()
        {
            // Act
            var response = await _commandHandler.ExecuteAsync(_command);

            // Assert
            response.Status.Should().Be(RequestStatus.Success);
        }

        [Test]
        public async Task Should_CloseBlock()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.CloseBlock())
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_NotCallAnyOtherServiceMethod()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.OpenBlock(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _logService.OpenScope()).MustNotHaveHappened();
            A.CallTo(() => _logService.CloseScope()).MustNotHaveHappened();
            A.CallTo(() => _logService.AddEntry(A<string>._)).MustNotHaveHappened();
        }
    }
}
