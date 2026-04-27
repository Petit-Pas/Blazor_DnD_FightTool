using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartNextRound;

/// <summary>
///     Sub-command that advances the round counter and logs the round change.
/// </summary>
public class StartNextRoundCommand : CommandBase
{
    /// <summary>
    ///     The round number before the increment. Set by the handler during <c>ExecuteAsync</c> for undo.
    /// </summary>
    public int PreviousRound { get; set; }
}
