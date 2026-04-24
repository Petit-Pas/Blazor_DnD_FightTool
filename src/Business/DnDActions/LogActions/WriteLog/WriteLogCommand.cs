using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.LogActions.WriteLog;

/// <summary>
///     Sub-command that writes a single log entry via <see cref="Domain.Logs.IDnDLogService"/>.
///     Dispatched as a sub-command of parent commands via <c>SendAsSubCommandAsync</c>.
/// </summary>
public class WriteLogCommand : CommandBase
{
    /// <summary>
    ///     Creates a new write-log command.
    /// </summary>
    /// <param name="content">The tokenized text to write.</param>
    public WriteLogCommand(string content)
    {
        Content = content;
    }

    /// <summary>
    ///     The tokenized text with BBCode-like formatting tags.
    /// </summary>
    public string Content { get; }

    /// <summary>
    ///     <c>null</c> before execution; set to the created <see cref="Domain.Logs.LogEntry.Id"/> after execution.
    /// </summary>
    public Guid? LogEntryId { get; set; }
}
