using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Business.DnDActions.TurnActions.SetCurrentFighter;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.TurnTracking;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;

/// <summary>
///     Orchestrator for <see cref="RemoveFromFightCommand"/>.
///     Validates the fighter exists, optionally clears the active turn selection via
///     <see cref="SetCurrentFighterCommand"/>, delegates the actual removal to
///     <see cref="RemoveFromFightAtomicCommand"/>, and emits a log entry.
///     Undo cascades automatically to all sub-commands via <c>base.UndoAsync</c>.
/// </summary>
public class RemoveFromFightCommandHandler : CommandHandlerBase<RemoveFromFightCommand>
{
    private readonly IFightContext _fightContext;
    private readonly ICombatTurnService _combatTurnService;

    public RemoveFromFightCommandHandler(
        IUndoableMediator mediator,
        IFightContext fightContext,
        ICombatTurnService combatTurnService) : base(mediator)
    {
        _fightContext = fightContext;
        _combatTurnService = combatTurnService;
    }

    /// <inheritdoc />
    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(RemoveFromFightCommand command)
    {
        var fighter = _fightContext[command.FighterId];
        if (fighter is null)
        {
            return new CommandResponse(RequestStatus.Failed);
        }

        if (_combatTurnService.CurrentTurnFighter?.Id == fighter.Id)
        {
            await _mediator.SendAsSubCommandAsync(
                new SetCurrentFighterCommand(null),
                parentCommand: command);
        }

        await _mediator.SendAsSubCommandAsync(
            new RemoveFromFightAtomicCommand(command.FighterId),
            parentCommand: command);

        await LogRemovedFighter(fighter.Name, command);

        return CommandResponse.Success();
    }

    /// <inheritdoc />
    public override Task UndoAsync(RemoveFromFightCommand command)
    {
        return base.UndoAsync(command);
    }

    /// <inheritdoc />
    public async override Task RedoAsync(RemoveFromFightCommand command)
    {
        ClearSubCommands(command);
        await ExecuteAsync(command);
    }

    private async Task LogRemovedFighter(string fighterName, RemoveFromFightCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]{fighterName}[/b] left the fight"),
            parentCommand: command);
    }
}
