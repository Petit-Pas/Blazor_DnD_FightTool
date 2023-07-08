using DnDEntities.AbilityScores;
using DnDEntities.Characters;

namespace Fight.Savings;

public class SaveRollResult
{
    /// <summary>
    ///     Rolled result, does not contain bonuses
    /// </summary>
    public int RolledResult { get; set; }

    /// <summary>
    ///     Can be 0 if inherited from the caster
    ///     // TODO could this be recalculated ? migth be handy
    /// </summary>
    public int Target { get; set; }

    /// <summary>
    ///     The ability targeted by the save
    /// </summary>
    public AbilityEnum Ability { get; set; }

    public virtual bool IsSuccesfull(Character targetCharacter, Character casterCharacter)
    {
        // TODO When characters have a saving throw implemented, the 2nd branch of this ternary should be updated to use it.
        // TODO also update unit test
        var target = Target != 0 ? Target : 15;

        var modifier = targetCharacter.AbilityScores.GetModifierWithMastery(Ability);

        return RolledResult + modifier.Modifier >= target;
    }
}
