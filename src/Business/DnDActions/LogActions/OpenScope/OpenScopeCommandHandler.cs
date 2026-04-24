using DnDFightTool.Domain.Logs;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.LogActions.OpenScope;

/// <summary>
///     Handler for <see cref="OpenScopeCommand"/>.
///     Increments the log indent level on execute; undo is a no-op.
/// </summary>
public class OpenScopeCommandHandler : CommandHandlerBase<OpenScopeCommand>
{
    private readonly IDnDLogService _logService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public OpenScopeCommandHandler(IUndoableMediator mediator, IDnDLogService logService) : base(mediator)
    {
        _logService = logService;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(OpenScopeCommand command)
    {
        _logService.OpenScope();
        return Task.FromResult(CommandResponse.Success());
    }
}
