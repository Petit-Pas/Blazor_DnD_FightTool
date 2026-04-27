using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.LogActions.CloseBlock;

/// <summary>
///     Sub-command that closes the current log block via <see cref="Domain.Logs.IDnDLogService"/>.
///     Stores the closed block's ID so undo can re-open it.
/// </summary>
public class CloseBlockCommand : CommandBase
{
    /// <summary>
    ///     The ID of the block that was closed. Set by the handler during <c>ExecuteAsync</c>.
    /// </summary>
    public Guid ClosedBlockId { get; set; }
}
