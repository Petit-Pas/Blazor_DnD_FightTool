using DnDEntities.Characters;
using Fight;
using UndoableMediator.Commands;

namespace DnDActions;

public class TargetCommandBase : CommandBase
{
    public TargetCommandBase(Guid targetId)
    {
        TargetId = targetId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    protected Guid TargetId { get; }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the character easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the affected HitPoints </returns>
    public Character GetTarget(IFightContext fightContext) => fightContext.GetCharacterById(TargetId) ?? throw new InvalidOperationException($"Could not get target for {GetType()}.");
}