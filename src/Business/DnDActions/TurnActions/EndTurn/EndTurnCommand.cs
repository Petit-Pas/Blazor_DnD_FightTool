using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.TurnActions.EndTurn;

/// <summary>
///     Sub-command that ends the current fighter's turn.
///     Dispatches <see cref="LogActions.CloseBlock.CloseBlockCommand"/> to close the turn's log block.
/// </summary>
public class EndTurnCommand : CommandBase
{
    /// <summary>
    ///     Initializes a new instance with the identifier of the fighter whose turn is ending.
    /// </summary>
    public EndTurnCommand(Guid fighterId)
    {
        FighterId = fighterId;
    }

    /// <summary>
    ///     Identifier of the fighter whose turn is ending.
    /// </summary>
    public Guid FighterId { get; set; }
}
