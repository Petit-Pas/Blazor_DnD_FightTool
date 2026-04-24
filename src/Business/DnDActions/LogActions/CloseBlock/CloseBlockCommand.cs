using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.LogActions.CloseBlock;

/// <summary>
///     Sub-command that closes the current log block via <see cref="Domain.Logs.IDnDLogService"/>.
///     Undo is a no-op: block visibility is governed by its entry sub-commands.
/// </summary>
public class CloseBlockCommand : CommandBase
{
}
