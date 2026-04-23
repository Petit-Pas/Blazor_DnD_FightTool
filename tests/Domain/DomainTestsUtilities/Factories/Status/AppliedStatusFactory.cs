using DnDFightTool.Domain.Fight;

namespace DomainTestsUtilities.Factories.Status;

public static class AppliedStatusFactory
{
    public static AppliedStatus BuildAppliedStatus(Guid? casterId = null, Guid? targetId = null, string? name = null)
    {
        return new AppliedStatus(casterId ?? Guid.NewGuid(), targetId ?? Guid.NewGuid(), name ?? "appliedStatusName");
    }
}
