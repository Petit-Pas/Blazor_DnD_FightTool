using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.FightActions.RemoveFromFight;

/// <summary>
///     Removes a fighter from the fight. Undoable: undo restores the fighter from the <see cref="IFightContext"/> internal stash.
///     If the removed fighter is the currently-active turn fighter, a sub-command nulls the current selection.
/// </summary>
public class RemoveFromFightCommand : CommandBase
{
    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="fighterId">The id of the fighter to remove.</param>
    public RemoveFromFightCommand(Guid fighterId)
    {
        FighterId = fighterId;
    }

    /// <summary>
    ///     The id of the fighter to remove.
    /// </summary>
    public Guid FighterId { get; }
}
