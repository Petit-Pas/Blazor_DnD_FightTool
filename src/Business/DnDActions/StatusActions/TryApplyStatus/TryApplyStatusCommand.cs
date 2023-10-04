using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Saves;
using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight;

namespace DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;

public class TryApplyStatusCommand : CasterTargetCommandBase
{
    public TryApplyStatusCommand(Guid casterId, Guid targetId, Guid statusId) : base(casterId, targetId)
    {
        StatusId = statusId;
    }

    public Guid StatusId { get; }

    /// <summary>
    ///     Set by the command handler
    /// </summary>
    internal SaveRollResult? SaveRollResult { get; set; }
}
