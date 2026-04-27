using DnDFightTool.Domain.Fight.TurnTracking;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.TurnActions.SetCurrentFighter;

/// <summary>
///     Handler for <see cref="SetCurrentFighterCommand"/>.
///     Sets the current turn fighter. Leaf command — no sub-commands.
/// </summary>
public class SetCurrentFighterCommandHandler : CommandHandlerBase<SetCurrentFighterCommand>
{
    private readonly ICombatTurnService _combatTurnService;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public SetCurrentFighterCommandHandler(IUndoableMediator mediator, ICombatTurnService combatTurnService) : base(mediator)
    {
        _combatTurnService = combatTurnService;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(SetCurrentFighterCommand command)
    {
        command.PreviousFighterId = _combatTurnService.CurrentTurnFighter?.Id;
        _combatTurnService.SetCurrentTurnFighter(command.FighterId);

        return Task.FromResult(CommandResponse.Success());
    }

    /// <inheritdoc />
    public override Task UndoAsync(SetCurrentFighterCommand command)
    {
        _combatTurnService.SetCurrentTurnFighter(command.PreviousFighterId);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task RedoAsync(SetCurrentFighterCommand command)
    {
        _combatTurnService.SetCurrentTurnFighter(command.FighterId);
        return Task.CompletedTask;
    }
}
