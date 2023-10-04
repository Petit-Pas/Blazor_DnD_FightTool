using DnDFightTool.Domain.DnDEntities.Saves;

namespace DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;

public class ApplyStatusCommand : CasterTargetCommandBase
{
    public ApplyStatusCommand(Guid casterId, Guid targetId, Guid statusId, SaveRollResult? saveRollResult) : base(casterId, targetId)
    {
        StatusId = statusId;
        SaveRollResult = saveRollResult;
    }

    public Guid StatusId { get; }
    public SaveRollResult? SaveRollResult { get; }
}