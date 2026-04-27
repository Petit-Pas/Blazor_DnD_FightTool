using DnDFightTool.Domain.Logs;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.LogActions.OpenBlock;

/// <summary>
///     Handler for <see cref="OpenBlockCommand"/>.
///     Opens a log block on execute; undo is a no-op because the block disappears
///     automatically when all its entry sub-commands are hidden.
/// </summary>
public class OpenBlockCommandHandler : CommandHandlerBase<OpenBlockCommand>
{
    private readonly IDnDLogService _logService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public OpenBlockCommandHandler(IUndoableMediator mediator, IDnDLogService logService) : base(mediator)
    {
        _logService = logService;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(OpenBlockCommand command)
    {
        _logService.OpenBlock(command.Name);
        return Task.FromResult(CommandResponse.Success());
    }

    /// <inheritdoc />
    public override Task UndoAsync(OpenBlockCommand command)
    {
        _logService.CloseBlock();
        return Task.CompletedTask;
    }
}
