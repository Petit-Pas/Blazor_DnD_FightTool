using DnDFightTool.Business.DnDActions.LogActions.WriteLog;
using DnDFightTool.Domain.Fight.TurnTracking;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartNextRound;

// TODO: Violates orchestrator/atomic rule — mixes direct state mutation (_combatTurnService.SetCurrentRound)
// with sub-commands (WriteLog) and both direct undo and base.UndoAsync. Refactor: extract the mutation
// into an atomic sub-command and make this handler a pure orchestrator. See commands.instructions.md.

/// <summary>
///     Handler for <see cref="StartNextRoundCommand"/>.
///     Increments the round counter and logs the round header.
/// </summary>
public class StartNextRoundCommandHandler : CommandHandlerBase<StartNextRoundCommand>
{
    private readonly ICombatTurnService _combatTurnService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public StartNextRoundCommandHandler(IUndoableMediator mediator, ICombatTurnService combatTurnService) : base(mediator)
    {
        _combatTurnService = combatTurnService;
    }

    /// <inheritdoc />
    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(StartNextRoundCommand command)
    {
        command.PreviousRound = _combatTurnService.CurrentRound;
        _combatTurnService.SetCurrentRound(command.PreviousRound + 1);

        await LogRoundHeader(command);

        return CommandResponse.Success();
    }

    /// <inheritdoc />
    public override Task UndoAsync(StartNextRoundCommand command)
    {
        _combatTurnService.SetCurrentRound(command.PreviousRound);
        return base.UndoAsync(command);
    }

    /// <inheritdoc />
    public override async Task RedoAsync(StartNextRoundCommand command)
    {
        _combatTurnService.SetCurrentRound(command.PreviousRound + 1);
        await base.RedoAsync(command);
    }

    private async Task LogRoundHeader(StartNextRoundCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new WriteLogCommand($"[b]Round {_combatTurnService.CurrentRound}[/b]"),
            parentCommand: command);
    }
}
