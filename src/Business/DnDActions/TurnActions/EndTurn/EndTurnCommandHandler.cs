using DnDFightTool.Business.DnDActions.LogActions.CloseBlock;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.TurnActions.EndTurn;

/// <summary>
///     Handler for <see cref="EndTurnCommand"/>.
///     Closes the current turn's log block. Undo is handled by the sub-command cascade.
/// </summary>
public class EndTurnCommandHandler : CommandHandlerBase<EndTurnCommand>
{
    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public EndTurnCommandHandler(IUndoableMediator mediator) : base(mediator)
    {
    }

    /// <inheritdoc />
    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(EndTurnCommand command)
    {
        await CloseTurnLog(command);
        return CommandResponse.Success();
    }

    private async Task CloseTurnLog(EndTurnCommand command)
    {
        await _mediator.SendAsSubCommandAsync(new CloseBlockCommand(), parentCommand: command);
    }
}
