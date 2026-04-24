using DnDFightTool.Domain.Logs;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.LogActions.WriteLog;

/// <summary>
///     Handler for <see cref="WriteLogCommand"/>.
///     Adds a log entry on execute, hides on undo, shows on redo.
/// </summary>
public class WriteLogCommandHandler : CommandHandlerBase<WriteLogCommand>
{
    private readonly IDnDLogService _logService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public WriteLogCommandHandler(IUndoableMediator mediator, IDnDLogService logService) : base(mediator)
    {
        _logService = logService;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(WriteLogCommand command)
    {
        command.LogEntryId = _logService.AddEntry(command.Content);
        return Task.FromResult(CommandResponse.Success());
    }

    /// <inheritdoc />
    public override Task UndoAsync(WriteLogCommand command)
    {
        _logService.Hide(command.LogEntryId!.Value);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task RedoAsync(WriteLogCommand command)
    {
        _logService.Show(command.LogEntryId!.Value);
        return Task.CompletedTask;
    }
}
