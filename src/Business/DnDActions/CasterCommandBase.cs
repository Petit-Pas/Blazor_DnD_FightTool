using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions;

public class CasterCommandBase : CommandBase
{
    public CasterCommandBase(Guid casterId)
    {
        CasterId = casterId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid CasterId { get; }
}
