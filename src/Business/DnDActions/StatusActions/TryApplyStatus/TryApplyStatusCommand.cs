using DnDFightTool.Domain.DnDEntities.Saves;

namespace DnDFightTool.Business.DnDActions.StatusActions.TryApplyStatus;

/// <summary>
///     A command to try to apply a status to a target. Rolls and all will be calculated further down the line.
/// </summary>
public class TryApplyStatusCommand : CasterTargetCommandBase
{
    public TryApplyStatusCommand(Guid casterId, Guid targetId, Guid statusId) : base(casterId, targetId)
    {
        StatusId = statusId;
    }

    /// <summary>
    ///     Status of the id to try to apply
    /// </summary>
    public Guid StatusId { get; }

    /// <summary>
    ///     Set by the command handler
    /// </summary>
    internal SaveRollResult? SaveRollResult { get; set; }

    /// <summary>
    ///     Set by the command handler to keep track to check if the status was updated between a DO and an UNDO
    /// </summary>
    public string StatusHash { get; set; } = "";
}
