using System;
using System.Threading.Tasks;
using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Logs;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDActionsTests.LogActions;

[TestFixture]
internal class WriteLogCommandHandlerTests
{
    private IUndoableMediator _mediator = null!;
    private IDnDLogService _logService = null!;

    private WriteLogCommand _command = null!;
    private WriteLogCommandHandler _commandHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mediator = A.Fake<IUndoableMediator>(options => options.Strict());
        _logService = A.Fake<IDnDLogService>(options => options.Strict());

        _command = new WriteLogCommand("Test log entry");
        _commandHandler = new WriteLogCommandHandler(_mediator, _logService);

        var fakeEntryId = Guid.NewGuid();
        A.CallTo(() => _logService.AddEntry(A<string>._))
            .Returns(fakeEntryId);
        A.CallTo(() => _logService.Hide(A<Guid>._)).DoesNothing();
        A.CallTo(() => _logService.Show(A<Guid>._)).DoesNothing();
    }

    [TestFixture]
    private class ExecuteTests : WriteLogCommandHandlerTests
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
        public async Task Should_CallAddEntry_WithContent()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            A.CallTo(() => _logService.AddEntry("Test log entry"))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Should_StoreLogEntryId()
        {
            // Act
            await _commandHandler.ExecuteAsync(_command);

            // Assert
            _command.LogEntryId.Should().NotBeNull();
        }
    }

    [TestFixture]
    private class UndoTests : WriteLogCommandHandlerTests
    {
        [Test]
        public async Task Should_HideEntry()
        {
            // Arrange
            await _commandHandler.ExecuteAsync(_command);
            var entryId = _command.LogEntryId!.Value;

            // Act
            await _commandHandler.UndoAsync(_command);

            // Assert
            A.CallTo(() => _logService.Hide(entryId))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    private class RedoTests : WriteLogCommandHandlerTests
    {
        [Test]
        public async Task Should_ShowEntry()
        {
            // Arrange
            await _commandHandler.ExecuteAsync(_command);
            var entryId = _command.LogEntryId!.Value;

            // Act
            await _commandHandler.RedoAsync(_command);

            // Assert
            A.CallTo(() => _logService.Show(entryId))
                .MustHaveHappenedOnceExactly();
        }
    }

    [TestFixture]
    private class FullCycleTests : WriteLogCommandHandlerTests
    {
        [Test]
        public async Task Should_Execute_Then_Undo_Hides_Then_Redo_Shows()
        {
            // Arrange
            await _commandHandler.ExecuteAsync(_command);
            var entryId = _command.LogEntryId!.Value;

            // Act - Undo
            await _commandHandler.UndoAsync(_command);

            // Assert - Hide was called
            A.CallTo(() => _logService.Hide(entryId))
                .MustHaveHappenedOnceExactly();

            // Act - Redo
            await _commandHandler.RedoAsync(_command);

            // Assert - Show was called
            A.CallTo(() => _logService.Show(entryId))
                .MustHaveHappenedOnceExactly();
        }
    }
}
