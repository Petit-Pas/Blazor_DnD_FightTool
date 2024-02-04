namespace DnDFightTool.Domain.DnDEntities.Statuses;

/// <summary>
///     A simple record for a status that is applied to a target by a caster
///     Does not hold any information about the status itself, only who applied it to whom
/// </summary>
/// <param name="CasterId"></param>
/// <param name="TargetId"></param>
/// <param name="Name"></param>
public record AppliedStatus(Guid CasterId, Guid TargetId, string Name)
{
    /// <summary>
    ///     A unique non meaningful id for this application of the status
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();
}
