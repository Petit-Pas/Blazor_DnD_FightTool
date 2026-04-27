using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.TurnActions.StartCombat;

/// <summary>
///     Sub-command that starts combat by initializing the turn order from the current fighters.
///     Throws if combat is already in progress.
/// </summary>
public class StartCombatCommand : CommandBase
{
}
