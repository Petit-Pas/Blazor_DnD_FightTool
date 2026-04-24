using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.LogActions.OpenScope;

/// <summary>
///     Sub-command that increments the log indent level via <see cref="Domain.Logs.IDnDLogService"/>.
///     Undo is a no-op: indent is baked into each entry at creation time.
/// </summary>
public class OpenScopeCommand : CommandBase
{
}
