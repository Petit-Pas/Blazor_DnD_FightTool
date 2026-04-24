using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.OpenScope;
using DnDFightTool.Domain.Logs;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.LogActions;

[TestFixture]
internal class OpenScopeCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IDnDLogService _logService = null!;
    private OpenScopeCommand _command = null!;
    private OpenScopeCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _logService = A.Fake<IDnDLogService>();
        _command = new OpenScopeCommand();
        _commandHandler = new OpenScopeCommandHandler(_mediator, _logService);
    }

    [TestFixture]
    private class ExecuteTests : OpenScopeCommandHandlerTests
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
        public async Task Should_OpenScope()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.OpenScope())
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
            A.CallTo(() => _logService.CloseScope()).MustNotHaveHappened();
            A.CallTo(() => _logService.AddEntry(A<string>._)).MustNotHaveHappened();
        }
    }
}
