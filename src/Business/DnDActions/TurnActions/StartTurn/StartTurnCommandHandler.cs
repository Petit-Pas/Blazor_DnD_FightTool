using DnDFightTool.Business.DnDActions.LogActions.OpenBlock;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartTurn;

/// <summary>
///     Handler for <see cref="StartTurnCommand"/>.
///     Opens a log block for the fighter identified by <see cref="StartTurnCommand.FighterId"/>.
///     Undo is handled by the sub-command cascade.
/// </summary>
public class StartTurnCommandHandler : CommandHandlerBase<StartTurnCommand>
{
    private readonly IFightContext _fightContext;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public StartTurnCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    /// <inheritdoc />
    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(StartTurnCommand command)
    {
        var fighter = _fightContext[command.FighterId]
            ?? throw new InvalidOperationException($"{nameof(StartTurnCommandHandler)}: Fighter {command.FighterId} not found.");

        await OpenTurnLog(command, fighter.Name);

        return CommandResponse.Success();
    }

    private async Task OpenTurnLog(StartTurnCommand command, string fighterName)
    {
        await _mediator.SendAsSubCommandAsync(
            new OpenBlockCommand($"{fighterName}'s turn"),
            parentCommand: command);
    }
}
