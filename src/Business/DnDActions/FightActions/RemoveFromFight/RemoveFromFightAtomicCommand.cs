using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;

/// <summary>
///     Atomic command: evicts a fighter from <see cref="DnDFightTool.Domain.Fight.IFightContext"/>.
///     Intended as a sub-command of <see cref="DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight.RemoveFromFightCommand"/>.
///     Undo re-inserts the fighter from the internal removed-fighter stash via <see cref="DnDFightTool.Domain.Fight.IFightContext.Restore"/>.
/// </summary>
public class RemoveFromFightAtomicCommand : CommandBase
{
    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="fighterId">The <see cref="DnDFightTool.Domain.Fight.Characters.IFightingCharacter.Id"/> of the fighter to evict.</param>
    public RemoveFromFightAtomicCommand(Guid fighterId)
    {
        FighterId = fighterId;
    }

    /// <summary>
    ///     The id of the fighter to evict.
    /// </summary>
    public Guid FighterId { get; }
}
