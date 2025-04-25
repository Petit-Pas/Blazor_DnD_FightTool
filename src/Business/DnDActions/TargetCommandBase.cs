using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions;

/// <summary>
///    Base class for commands that affect a target
/// </summary>
public class TargetCommandBase : CommandBase
{
    public TargetCommandBase(Guid targetId)
    {
        TargetId = targetId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid TargetId { get; }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the target easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> the target affected by the command </returns>
    public Character GetTarget(IFightContext fightContext)
    {
        return fightContext.GetCharacterById(TargetId) ?? throw new InvalidOperationException($"Could not get target for {GetType()}.");
    }
}