using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;
using UndoableMediator.Mediators;
using UndoableMediator.Requests;

namespace DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;

/// <summary>
///     Atomic handler for <see cref="RemoveFromFightAtomicCommand"/>.
///     Execute removes the fighter from <see cref="IFightContext"/>;
///     Undo restores it from the internal stash via <see cref="IFightContext.Restore"/>.
///     No sub-commands — undo/redo do not cascade.
/// </summary>
public class RemoveFromFightAtomicCommandHandler : CommandHandlerBase<RemoveFromFightAtomicCommand>
{
    private readonly IFightContext _fightContext;

    public RemoveFromFightAtomicCommandHandler(IUndoableMediator mediator, IFightContext fightContext) : base(mediator)
    {
        _fightContext = fightContext;
    }

    /// <inheritdoc />
    public override Task<ICommandResponse<NoResponse>> ExecuteAsync(RemoveFromFightAtomicCommand command)
    {
        var fighter = _fightContext[command.FighterId]!;
        _fightContext.Remove(fighter);
        return Task.FromResult<ICommandResponse<NoResponse>>(CommandResponse.Success());
    }

    /// <inheritdoc />
    public override Task UndoAsync(RemoveFromFightAtomicCommand command)
    {
        _fightContext.Restore(command.FighterId);
        return Task.CompletedTask;
    }
}
