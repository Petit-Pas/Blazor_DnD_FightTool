using DnDFightTool.Domain.Logs;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.LogActions.CloseBlock;

/// <summary>
///     Handler for <see cref="CloseBlockCommand"/>.
///     Closes the current log block on execute; undo is a no-op.
/// </summary>
public class CloseBlockCommandHandler : CommandHandlerBase<CloseBlockCommand>
{
    private readonly IDnDLogService _logService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public CloseBlockCommandHandler(IUndoableMediator mediator, IDnDLogService logService) : base(mediator)
    {
        _logService = logService;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(CloseBlockCommand command)
    {
        command.ClosedBlockId = _logService.CloseBlock();
        return Task.FromResult(CommandResponse.Success());
    }

    /// <inheritdoc />
    public override Task UndoAsync(CloseBlockCommand command)
    {
        _logService.ReopenBlock(command.ClosedBlockId);
        return Task.CompletedTask;
    }
}
