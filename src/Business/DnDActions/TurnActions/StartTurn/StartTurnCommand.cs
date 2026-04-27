using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartTurn;

/// <summary>
///     Sub-command that starts a fighter's turn by opening their log block.
///     Dispatches <see cref="LogActions.OpenBlock.OpenBlockCommand"/>.
/// </summary>
public class StartTurnCommand : CommandBase
{
    /// <summary>
    ///     Initializes a new instance with the identifier of the fighter whose turn is starting.
    /// </summary>
    public StartTurnCommand(Guid fighterId)
    {
        FighterId = fighterId;
    }

    /// <summary>
    ///     Identifier of the fighter whose turn is starting.
    /// </summary>
    public Guid FighterId { get; set; }
}
