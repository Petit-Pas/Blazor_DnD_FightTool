using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

namespace DnDFightTool.Domain.Fight;

public class AppliedStatusCollection : IAppliedStatusCollection
{
    public event AppliedStatusUpdatedEventHandler? AppliedStatusUpdated;
    public void NotifyAppliedStatusUpdated(Guid affectedCharacterId)
    {
        AppliedStatusUpdated?.Invoke(this, new AppliedStatusUpdatedEventArgs(affectedCharacterId));
    }
    private List<AppliedStatus> _appliedStatuses;

    public AppliedStatusCollection()
    {
        _appliedStatuses = new List<AppliedStatus>();
    }

    public void Add(AppliedStatus appliedStatus)
    {
        _appliedStatuses.Add(appliedStatus);

        NotifyAppliedStatusUpdated(appliedStatus.TargetId);
    }


    public IEnumerable<AppliedStatus> GetStatusAppliedTo(Guid affectedCharacterId)
    {
        return _appliedStatuses.Where(x => x.TargetId == affectedCharacterId);
    }
}
