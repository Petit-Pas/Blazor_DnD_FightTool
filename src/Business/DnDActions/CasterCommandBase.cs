using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions;

public class CasterCommandBase : CommandBase
{
    public CasterCommandBase(Guid casterId)
    {
        CasterId = casterId;
    }

    /// <summary>
    ///     Guid of the character affected by the command
    /// </summary>
    public Guid CasterId { get; }

    /// <summary>
    ///     Simple helper method for the command handlers, to get the caster easily
    /// </summary>
    /// <param name="fightContext"> A required dependency for the method. Since the handlers is injected through DI, its easier for it to provide the service. </param>
    /// <returns> The caster executing the command </returns>
    public FightingCharacter GetCaster(IFightContext fightContext)
    {
        return fightContext[CasterId];
    }
}
