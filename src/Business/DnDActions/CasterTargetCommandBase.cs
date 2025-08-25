using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions;

public class CasterTargetCommandBase : CommandBase
{
    public CasterTargetCommandBase(Guid casterId, Guid targetId)
    {
        TargetId = targetId;
        CasterId = casterId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid TargetId { get; }

    /// <summary>
    ///     Guid of the character actually executing the action
    /// </summary>
    public Guid CasterId { get; }
}
