using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.TurnTracking;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartCombat;

/// <summary>
///     Handler for <see cref="StartCombatCommand"/>.
///     Initializes <see cref="ICombatTurnService"/> from the current fighters.
///     Throws if combat is already in progress.
/// </summary>
public class StartCombatCommandHandler : CommandHandlerBase<StartCombatCommand>
{
    private readonly ICombatTurnService _combatTurnService;
    private readonly IFightContext _fightContext;

    /// <summary>
    ///     Creates a new handler instance.
    /// </summary>
    public StartCombatCommandHandler(IUndoableMediator mediator, ICombatTurnService combatTurnService, IFightContext fightContext) : base(mediator)
    {
        _combatTurnService = combatTurnService;
        _fightContext = fightContext;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(StartCombatCommand command)
    {
        if (_combatTurnService.IsStarted)
        {
            throw new InvalidOperationException($"{nameof(StartCombatCommandHandler)}: Cannot start combat while it is already in progress.");
        }

        _combatTurnService.Initialize(_fightContext.Fighters);

        return Task.FromResult(CommandResponse.Success());
    }

    /// <inheritdoc />
    public override Task UndoAsync(StartCombatCommand command)
    {
        // Undo is a full reset — combat was not started before this command ran.
        _combatTurnService.Initialize([]);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task RedoAsync(StartCombatCommand command)
    {
        _combatTurnService.Initialize(_fightContext.Fighters);
        return Task.CompletedTask;
    }
}
