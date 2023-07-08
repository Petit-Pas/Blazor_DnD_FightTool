using DnDEntities.Characters;
using Fight.Damage;

namespace Fight.MartialAttacks;

public class MartialAttackRollResult
{
    public int HitRoll { get; set; }
    public DamageRollResult[] DamageRolls { get; set; } = Array.Empty<DamageRollResult>();

    public Guid TargetId { get; set; }

    public virtual bool Hits(MartialAttackTemplate attackTemplate, Character targetCharacter, Character casterCharacter)
    {
        // TODO changing this boolean to a domain object might enable better logging in the 

        // Critical miss
        if (HitRoll == 1)
        {
            return false;
        }

        // Critical hit
        if (HitRoll == 20) 
        {
            return true;
        }

        // TODO missing bonuses to hit that would come from the query directly => should be a property here

        // TODO Should come from the template (maybe needing the caster, for bonuses such as + STR + MAS for instance
        var innateBonusToHit = 0;

        var totalAttackScore = HitRoll + innateBonusToHit;
        var totalDefenseScore = targetCharacter.ArmorClass.EffectiveAC;

        return totalAttackScore >= totalDefenseScore;
    }
}
