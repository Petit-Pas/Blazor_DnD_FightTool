using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions;

/// <summary>
///    Base class for commands that affect a target
/// </summary>
public class TargetCommandBase : CommandBase
{
    public TargetCommandBase(Guid targetId)
    {
        TargetId = targetId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid TargetId { get; }
}