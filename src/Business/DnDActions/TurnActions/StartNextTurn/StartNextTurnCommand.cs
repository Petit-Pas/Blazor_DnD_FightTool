using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartNextTurn;

/// <summary>
///     Orchestrator command for advancing to the next fighter's turn.
///     Handles both "Start Combat" (first press) and "Next Turn" (subsequent presses).
///     All state mutations are sub-commands — undo/redo cascades automatically.
/// </summary>
public class StartNextTurnCommand : CommandBase
{
}
