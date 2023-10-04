namespace DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

public delegate void AppliedStatusUpdatedEventHandler(object sender, AppliedStatusUpdatedEventArgs e);

public class AppliedStatusUpdatedEventArgs : EventArgs
{

    public AppliedStatusUpdatedEventArgs(Guid affectedCharacterId)
    {
        AffectedCharacterId = affectedCharacterId;
    }

    public Guid AffectedCharacterId { get; }
}
