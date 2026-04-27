using DnDFightTool.Business.DnDActions.TurnActions.EndTurn;
using DnDFightTool.Business.DnDActions.TurnActions.StartCombat;
using DnDFightTool.Business.DnDActions.TurnActions.SetCurrentFighter;
using DnDFightTool.Business.DnDActions.TurnActions.StartNextRound;
using DnDFightTool.Business.DnDActions.TurnActions.StartTurn;
using DnDFightTool.Domain.Fight.TurnTracking;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartNextTurn;

/// <summary>
///     Handler for <see cref="StartNextTurnCommand"/>.
///     Orchestrates end-of-turn, round advancement, and start-of-turn as sub-commands.
///     All <see cref="ICombatTurnService"/> mutations go through sub-commands so that
///     cascade redo replays them correctly.
/// </summary>
public class StartNextTurnCommandHandler : CommandHandlerBase<StartNextTurnCommand>
{
    private readonly ICombatTurnService _combatTurnService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public StartNextTurnCommandHandler(
        IUndoableMediator mediator,
        ICombatTurnService combatTurnService) : base(mediator)
    {
        _combatTurnService = combatTurnService;
    }

    /// <inheritdoc />
    public async override Task<ICommandResponse<NoResponse>> ExecuteAsync(StartNextTurnCommand command)
    {
        // 1. End current turn (if started)
        if (_combatTurnService.IsStarted)
        {
            await EndCurrentTurn(command);
        }

        // 2. Initialize on first turn
        var wasStarted = _combatTurnService.IsStarted;
        if (!wasStarted)
        {
            await StartCombat(command);
        }

        // 3. Get next fighter (read-only — no command needed)
        var nextFighter = _combatTurnService.GetNextFighter();

        // 4. Round advancement (last turn of round OR first turn)
        if (_combatTurnService.IsLastTurnOfRound() || !wasStarted)
        {
            await AdvanceRound(command);
        }

        // 5. Set current fighter
        await SetCurrentFighter(command, nextFighter.Id);

        // 6. Start the new turn (opens log block)
        await BeginTurn(command, nextFighter.Id);

        return CommandResponse.Success();
    }

    /// <inheritdoc />
    public override Task UndoAsync(StartNextTurnCommand command)
    {
        return base.UndoAsync(command);
    }

    private async Task EndCurrentTurn(StartNextTurnCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new EndTurnCommand(_combatTurnService.CurrentTurnFighter!.Id),
            parentCommand: command);
    }

    private async Task StartCombat(StartNextTurnCommand command)
    {
        await _mediator.SendAsSubCommandAsync(
            new StartCombatCommand(),
            parentCommand: command);
    }

    private async Task AdvanceRound(StartNextTurnCommand command)
    {
        await _mediator.SendAsSubCommandAsync(new StartNextRoundCommand(), parentCommand: command);
    }

    private async Task SetCurrentFighter(StartNextTurnCommand command, Guid fighterId)
    {
        await _mediator.SendAsSubCommandAsync(
            new SetCurrentFighterCommand(fighterId),
            parentCommand: command);
    }

    private async Task BeginTurn(StartNextTurnCommand command, Guid fighterId)
    {
        await _mediator.SendAsSubCommandAsync(
            new StartTurnCommand(fighterId),
            parentCommand: command);
    }
}
