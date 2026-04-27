using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.TurnActions.SetCurrentFighter;

/// <summary>
///     Sub-command that sets the current turn fighter on <see cref="Domain.Fight.TurnTracking.ICombatTurnService"/>.
/// </summary>
public class SetCurrentFighterCommand : CommandBase
{
    /// <summary>
    ///     Initializes a new instance with the fighter to make current.
    /// </summary>
    public SetCurrentFighterCommand(Guid fighterId)
    {
        FighterId = fighterId;
    }

    /// <summary>
    ///     The ID of the fighter to set as the current turn fighter.
    /// </summary>
    public Guid FighterId { get; }

    /// <summary>
    ///     The ID of the fighter that was current before this command ran. <c>null</c> if none.
    ///     Set by the handler during <c>ExecuteAsync</c> for undo.
    /// </summary>
    public Guid? PreviousFighterId { get; set; }
}
