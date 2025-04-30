using DnDFightTool.Domain.DnDEntities.Statuses;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

namespace DnDFightTool.Domain.Fight;

public interface IAppliedStatusRepository
{
    /// <summary>
    ///     Add an applied status to the repository
    /// </summary>
    /// <param name="appliedStatus"></param>
    void Add(AppliedStatus appliedStatus);

    /// <summary>
    ///     Remove an applied status from the repository if it exists
    /// </summary>
    /// <param name="appliedStatusId"></param>
    void RemoveIfExists(Guid appliedStatusId);

    /// <summary>
    ///     Warns whenever a status is added or removed from the repository
    /// </summary>
    event AppliedStatusUpdatedEventHandler AppliedStatusUpdated;

    /// <summary>
    ///     Get all the statuses applied to a given character
    /// </summary>
    /// <param name="affectedCharacterId"></param>
    /// <returns></returns>
    IEnumerable<AppliedStatus> GetStatusAppliedTo(Guid affectedCharacterId);
}