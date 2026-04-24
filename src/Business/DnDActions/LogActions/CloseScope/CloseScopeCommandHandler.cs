using DnDFightTool.Domain.Logs;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.LogActions.CloseScope;

/// <summary>
///     Handler for <see cref="CloseScopeCommand"/>.
///     Decrements the log indent level on execute; undo is a no-op.
/// </summary>
public class CloseScopeCommandHandler : CommandHandlerBase<CloseScopeCommand>
{
    private readonly IDnDLogService _logService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public CloseScopeCommandHandler(IUndoableMediator mediator, IDnDLogService logService) : base(mediator)
    {
        _logService = logService;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(CloseScopeCommand command)
    {
        _logService.CloseScope();
        return Task.FromResult(CommandResponse.Success());
    }
}
