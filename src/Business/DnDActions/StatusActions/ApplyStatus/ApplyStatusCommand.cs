using DnDFightTool.Domain.DnDEntities.Saves;

namespace DnDFightTool.Business.DnDActions.StatusActions.ApplyStatus;

public class ApplyStatusCommand : CasterTargetCommandBase
{
    public ApplyStatusCommand(Guid casterId, Guid targetId, Guid statusId, SaveRollResult? saveRollResult) : base(casterId, targetId)
    {
        StatusId = statusId;
        SaveRollResult = saveRollResult;
    }

    /// <summary>
    ///     The id of the applied status
    /// </summary>
    public Guid StatusId { get; }

    /// <summary>
    ///     The result of the save roll if there was one. 
    /// </summary>
    public SaveRollResult? SaveRollResult { get; }

    /// <summary>
    ///     Saved by the handler to find the status back when needed.
    /// </summary>
    public Guid AppliedStatusId { get; internal set; }
}