using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.OpenBlock;
using DnDFightTool.Domain.Logs;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.LogActions;

[TestFixture]
internal class OpenBlockCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IDnDLogService _logService = null!;
    private OpenBlockCommand _command = null!;
    private OpenBlockCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>();
        _logService = A.Fake<IDnDLogService>();
        _command = new OpenBlockCommand("Test Block");
        _commandHandler = new OpenBlockCommandHandler(_mediator, _logService);
    }

    [TestFixture]
    private class ExecuteTests : OpenBlockCommandHandlerTests
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
        public async Task Should_OpenBlock_WithCommandName()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.OpenBlock("Test Block"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_Store_OpenedBlockId()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            A.CallTo(() => _logService.OpenBlock(A<string>._)).Returns(expectedId);

            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _command.OpenedBlockId.Should().Be(expectedId);
        }

        [Test]
        public async Task Should_NotCallAnyOtherServiceMethod()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.CloseBlock()).MustNotHaveHappened();
            A.CallTo(() => _logService.OpenScope()).MustNotHaveHappened();
            A.CallTo(() => _logService.CloseScope()).MustNotHaveHappened();
            A.CallTo(() => _logService.AddEntry(A<string>._)).MustNotHaveHappened();
        }
    }

    [TestFixture]
    private class UndoTests : OpenBlockCommandHandlerTests
    {
        [Test]
        public async Task Should_CloseBlock()
        {
            // Act
            await _commandHandler.UndoAsync(_command);

            // Assert
            A.CallTo(() => _logService.CloseBlock())
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    private class RedoTests : OpenBlockCommandHandlerTests
    {
        [Test]
        public async Task Should_ReopenBlock_WithStoredId()
        {
            // Arrange
            var blockId = Guid.NewGuid();
            _command.OpenedBlockId = blockId;

            // Act
            await _commandHandler.RedoAsync(_command);

            // Assert
            A.CallTo(() => _logService.ReopenBlock(blockId))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_NotCallOpenBlock()
        {
            // Arrange
            _command.OpenedBlockId = Guid.NewGuid();

            // Act
            await _commandHandler.RedoAsync(_command);

            // Assert
            A.CallTo(() => _logService.OpenBlock(A<string>._)).MustNotHaveHappened();
        }
    }
}
