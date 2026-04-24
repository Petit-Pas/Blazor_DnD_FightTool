using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.CloseScope;
using DnDFightTool.Domain.Logs;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.LogActions;

[TestFixture]
internal class CloseScopeCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IDnDLogService _logService = null!;
    private CloseScopeCommand _command = null!;
    private CloseScopeCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _logService = A.Fake<IDnDLogService>();
        _command = new CloseScopeCommand();
        _commandHandler = new CloseScopeCommandHandler(_mediator, _logService);
    }

    [TestFixture]
    private class ExecuteTests : CloseScopeCommandHandlerTests
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
        public async Task Should_CloseScope()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.CloseScope())
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_NotCallAnyOtherServiceMethod()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.OpenBlock(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _logService.CloseBlock()).MustNotHaveHappened();
            A.CallTo(() => _logService.OpenScope()).MustNotHaveHappened();
            A.CallTo(() => _logService.AddEntry(A<string>._)).MustNotHaveHappened();
        }
    }
}
