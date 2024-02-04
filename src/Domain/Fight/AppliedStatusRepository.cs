using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

namespace DnDFightTool.Domain.Fight;

/// <summary>
///    Repository for applied statuses
/// </summary>
public class AppliedStatusRepository : Dictionary<Guid, AppliedStatus>, IAppliedStatusRepository
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public AppliedStatusRepository()
    {
    }

    /// <inheritdoc/>
    public event AppliedStatusUpdatedEventHandler? AppliedStatusUpdated;
    private void NotifyAppliedStatusUpdated(AppliedStatus modifiedStatus)
    {
        AppliedStatusUpdated?.Invoke(this, new AppliedStatusUpdatedEventArgs(modifiedStatus.TargetId));
    }

    /// <inheritdoc/>
    public void Add(AppliedStatus appliedStatus)
    {
        Add(appliedStatus.Id, appliedStatus);
        NotifyAppliedStatusUpdated(appliedStatus);
    }

    private AppliedStatus? GetStatusByStatusId(Guid appliedStatusId)
    {
        if (TryGetValue(appliedStatusId, out var status))
        {
            return status;
        }
        return default;
    }

    /// <inheritdoc/>
    public void RemoveIfExists(Guid appliedStatusId)
    {
        var status = GetStatusByStatusId(appliedStatusId);

        if (status != default)
        {
            Remove(status.Id);
            NotifyAppliedStatusUpdated(status);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<AppliedStatus> GetStatusAppliedTo(Guid affectedCharacterId)
    {
        return Values.Where(x => x.TargetId == affectedCharacterId);
    }
}
