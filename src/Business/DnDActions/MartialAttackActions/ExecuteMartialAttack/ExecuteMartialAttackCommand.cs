using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Fight.Characters;

namespace DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;

/// <summary>
///     Main command to execute a martial attack
/// </summary>
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
    ///     Also contains the Id of the target
    /// </summary>
    public MartialAttackRollResult? MartialAttackRollResult { get; set; }


    /// <summary>
    ///     Calculated during the execution of the command
    ///     If it changes between the execution and the redo, the attack will not be redone but rather re-queried.
    /// </summary>
    public string AttackTemplateHash { get; internal set; } = "";

    /// <summary>
    ///     Remember if the attack hit or not
    /// </summary>
    public bool AttackDidHit { get; internal set; }

    /// <summary>
    ///     Clears the cached state of the command, to force a re-query on the next execution.
    /// </summary>
    internal void ClearCachedState()
    {
        MartialAttackRollResult = null;
        AttackTemplateHash = "";
        AttackDidHit = false;
    }

    /// <summary>
    ///     Helper method to get the attack template from the caster's martial attacks
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal MartialAttackTemplate GetAttackTemplate(FightingCharacter caster)
    {
        // TODO This does not work for monsters, probably some issues with the duplication of the attacks when adding them to fight.
        return caster.MartialAttacks.GetTemplateByIdOrDefault(MartialAttackId) ?? throw new InvalidOperationException($"Could not get a martial attack of id {MartialAttackId} for character {caster.Id}");
    }
}
