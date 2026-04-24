using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.LogActions.OpenBlock;

/// <summary>
///     Sub-command that opens a new named log block via <see cref="Domain.Logs.IDnDLogService"/>.
///     Undo is a no-op: the block disappears visually when all its entry sub-commands are hidden.
/// </summary>
public class OpenBlockCommand : CommandBase
{
    /// <summary>
    ///     Creates a new open-block command.
    /// </summary>
    /// <param name="name">Display name for the block.</param>
    public OpenBlockCommand(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Display name of the block to open.
    /// </summary>
    public string Name { get; }
}
