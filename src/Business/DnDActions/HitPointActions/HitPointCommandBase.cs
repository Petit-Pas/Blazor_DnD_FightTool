using DnDFightTool.Domain.DnDEntities.HitPoint;
using DnDFightTool.Domain.Fight;

namespace DnDFightTool.Business.DnDActions.HitPointActions;

public class HitPointCommandBase : TargetCommandBase
{
    public HitPointCommandBase(Guid targetId) : base(targetId)
    {
    }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the hitPoints easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the affected HitPoints </returns>
    public HitPoints GetHitPoints(IFightContext fightContext)
    {
        return fightContext.GetCharacterById(TargetId)?.HitPoints ?? throw new InvalidOperationException($"Could not get hitPoints for {GetType()}.");
    }
}
