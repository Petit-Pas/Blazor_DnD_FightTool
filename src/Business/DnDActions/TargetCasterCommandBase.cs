using DnDEntities.Characters;
using Fight;
using UndoableMediator.Commands;

namespace DnDActions;

public class TargetCasterCommandBase : CommandBase
{
    public TargetCasterCommandBase(Guid targetId, Guid casterId)
    {
        TargetId = targetId;
        CasterId = casterId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    protected Guid TargetId { get; }

    /// <summary>
    ///     Guid of the character actually executing the action
    /// </summary>
    protected Guid CasterId { get; }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the character easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the target character </returns>
    public Character GetTarget(IFightContext fightContext) => fightContext.GetCharacterById(TargetId) ?? throw new InvalidOperationException($"Could not get target for {GetType()}.");

    /// <summary>
    ///     Simple helper method for the command handlers, to get the character easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the caster character </returns>
    public Character GetCaster(IFightContext fightContext) => fightContext.GetCharacterById(CasterId) ?? throw new InvalidOperationException($"Could not get caster for {GetType()}.");

}
