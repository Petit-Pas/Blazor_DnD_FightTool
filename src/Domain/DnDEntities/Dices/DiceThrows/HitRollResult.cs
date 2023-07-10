using DnDEntities.Characters;

namespace DnDEntities.Dices.DiceThrows;

public class HitRollResult : D20BaseRollResult
{
    public HitRollResult()
    {
    }

    public HitRollResult(ModifiersTemplate modifiersTemplate)
    {
        Modifiers = modifiersTemplate;
    }

    public ModifiersTemplate Modifiers { get; set; } = new ModifiersTemplate();


    public bool IsACriticalHit()
    {
        return Result == 20;
    }

    public bool IsACriticalMiss()
    {
        return Result == 0;
    }

    // TODO changing the returned boolean to a domain object might enable better logging in the consuming code (for crits for instance)
    public bool Hits(Character target, Character caster)
    {
        if (IsACriticalMiss())
        {
            return false;
        }
        if (IsACriticalHit())
        {
            return true;
        }

        var modifier = Modifiers.GetScoreModifier(caster);

        var totalAttackScore = modifier.ApplyTo(Result);
        var totalDefenseScore = target.ArmorClass.EffectiveAC;

        return totalAttackScore >= totalDefenseScore;
    }
}
