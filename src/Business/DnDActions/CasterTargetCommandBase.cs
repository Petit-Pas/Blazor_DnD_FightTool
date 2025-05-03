using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions;

public class CasterTargetCommandBase : CommandBase
{
    public CasterTargetCommandBase(Guid casterId, Guid targetId)
    {
        TargetId = targetId;
        CasterId = casterId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid TargetId { get; }

    /// <summary>
    ///     Guid of the character actually executing the action
    /// </summary>
    public Guid CasterId { get; }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the character easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the target character </returns>
    public Character GetTarget(IFightContext fightContext)
    {
        return fightContext.GetCharacterById(TargetId) ?? throw new InvalidOperationException($"Could not get target for {GetType()}.");
    }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the character easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the caster character </returns>
    public Character GetCaster(IFightContext fightContext)
    {
        return fightContext.GetCharacterById(CasterId) ?? throw new InvalidOperationException($"Could not get caster for {GetType()}.");
    }
}
