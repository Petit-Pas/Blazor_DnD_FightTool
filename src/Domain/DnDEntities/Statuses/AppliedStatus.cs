namespace DnDFightTool.Domain.DnDEntities.Statuses;

public record AppliedStatus(Guid CasterId, Guid TargetId, string Name)
{
    public Guid Id { get; } = Guid.NewGuid();
}
