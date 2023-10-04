using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

namespace DnDFightTool.Domain.Fight;

public class AppliedStatusCollection : IAppliedStatusCollection
{
    public event AppliedStatusUpdatedEventHandler? AppliedStatusUpdated;
    public void NotifyAppliedStatusUpdated(AppliedStatus modifiedStatus)
    {
        AppliedStatusUpdated?.Invoke(this, new AppliedStatusUpdatedEventArgs(modifiedStatus.TargetId));
    }
    private List<AppliedStatus> _appliedStatuses;

    public AppliedStatusCollection()
    {
        _appliedStatuses = new List<AppliedStatus>();
    }

    public void Add(AppliedStatus appliedStatus)
    {
        _appliedStatuses.Add(appliedStatus);

        NotifyAppliedStatusUpdated(appliedStatus);
    }

    private AppliedStatus? GetStatusByStatusId(Guid appliedStatusId)
    {
        return _appliedStatuses.FirstOrDefault(x => x.Id == appliedStatusId);
    }

    public void RemoveIfExists(Guid appliedStatusId)
    {
        var status = GetStatusByStatusId(appliedStatusId);

        if (status != null)
        {
            _appliedStatuses.Remove(status);
            NotifyAppliedStatusUpdated(status);
        }
    }


    public IEnumerable<AppliedStatus> GetStatusAppliedTo(Guid affectedCharacterId)
    {
        return _appliedStatuses.Where(x => x.TargetId == affectedCharacterId);
    }

}
