using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.LogActions.CloseScope;

/// <summary>
///     Sub-command that decrements the log indent level via <see cref="Domain.Logs.IDnDLogService"/>.
///     Undo is a no-op: indent is baked into each entry at creation time.
/// </summary>
public class CloseScopeCommand : CommandBase
{
}
