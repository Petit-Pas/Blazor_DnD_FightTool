namespace DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

public delegate void AppliedStatusUpdatedEventHandler(object sender, AppliedStatusUpdatedEventArgs e);

/// <summary>
///     An event for when a status is added or removed from the applied status repository
/// </summary>
public class AppliedStatusUpdatedEventArgs : EventArgs
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="affectedCharacterId"></param>
    public AppliedStatusUpdatedEventArgs(Guid affectedCharacterId)
    {
        AffectedCharacterId = affectedCharacterId;
    }

    /// <summary>
    ///     Unique, non meaningful id of the affected character
    /// </summary>
    public Guid AffectedCharacterId { get; }
}
