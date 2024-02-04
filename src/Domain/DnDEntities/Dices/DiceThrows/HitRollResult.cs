using DnDFightTool.Domain.DnDEntities.Characters;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

/// <summary>
///     Specialized class for result of hit roll (attacks or spells that requires an attack roll)
/// </summary>
public class HitRollResult : D20BaseRollResult
{
    /// <summary>
    ///     Empty ctor
    /// </summary>
    public HitRollResult()
    {
    }

    /// <summary>
    ///     Ctor that allows for modifiers to be passed
    /// </summary>
    /// <param name="modifiersTemplate"> modifiers Example: STR+MAS+1 </param>
    public HitRollResult(ModifiersTemplate modifiersTemplate)
    {
        Modifiers = modifiersTemplate;
    }

    /// <summary>
    ///     Modifiers to be applied to the roll
    /// </summary>
    public ModifiersTemplate Modifiers { get; set; } = new ModifiersTemplate();

    /// <summary>
    ///     Checks if the roll is a critical hit
    /// </summary>
    /// <returns></returns>
    public bool IsACriticalHit()
    {
        return Result == 20;
    }

    /// <summary>
    ///     Checks if the roll is a critical miss
    /// </summary>
    /// <returns></returns>
    public bool IsACriticalMiss()
    {
        return Result == 1;
    }

    // TODO changing the returned boolean to a domain object might enable better logging in the consuming code (for crits for instance)
    /// <summary>
    ///    Checks if the roll hits the target
    /// </summary>
    /// <param name="target"> the target of the attack </param>
    /// <param name="caster"> the character that attacks </param>
    /// <returns></returns>
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
