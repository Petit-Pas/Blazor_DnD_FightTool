using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;

public class ExecuteMartialAttackCommand : CasterCommandBase
{
    /// <summary>
    ///     Command to execute a martial attack
    /// </summary>
    /// <param name="casterId"> the id of the caster of the attack </param>
    /// <param name="martialAttackId"> the id of the attack to be executed by the caster. Using an Id allows for the template to be updated before redoing the attack. </param>
    public ExecuteMartialAttackCommand(Guid casterId, Guid martialAttackId) : base(casterId)
    {
        MartialAttackId = martialAttackId;
    }

    /// <summary>
    ///     The Id of the martial attack. 
    /// </summary>
    public Guid MartialAttackId { get; }

    /// <summary>
    ///     Calculated during the execution of the command
    /// </summary>
    public MartialAttackRollResult? MartialAttackRollResult { get; set; }

    /// <summary>
    ///     Calculated during the execution of the command
    /// </summary>
    public string Hash { get; internal set; }

    internal MartialAttackTemplate GetAttackTemplate(Character caster)
    {
        return caster.MartialAttacks.GetTemplateById(MartialAttackId) ?? throw new InvalidOperationException($"Could not get a martial attack of id {MartialAttackId} for character {caster.Id}");
    }
}
