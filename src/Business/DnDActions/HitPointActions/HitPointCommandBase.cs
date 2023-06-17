using DnDEntities.HitPoint;
using Fight;

namespace DnDActions.HitPointActions;

public class HitPointCommandBase : TargetedCommandBase
{
    public HitPointCommandBase(Guid targetId) : base(targetId)
    {
    }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the hitPoints easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the affected HitPoints </returns>
    public HitPoints? GetHitPoints(IFightContext fightContext)
    {
        return fightContext.GetCharacterById(TargetId)?.HitPoints;
    }
}
