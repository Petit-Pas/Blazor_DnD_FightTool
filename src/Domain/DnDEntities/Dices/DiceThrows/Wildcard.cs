using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

/// <summary>
///     Used to store a value that can be resolved depending on the character needing it.
///     EX: STR/DEX/INT/... or MAS for mastery for example.
/// </summary>
public record Wildcard(string Token)
{
    /// <summary>
    ///     Private method to check if that wildcard is an ability represented in <see cref="AbilityEnum" />
    /// </summary>
    /// <param name="abilityOut"></param>
    /// <returns></returns>
    private bool IsAnAbility(out AbilityEnum abilityOut)
    {
        foreach (var ability in Enum.GetValues<AbilityEnum>())
        {
            if (Token == ability.ShortName())
            {
                abilityOut = ability;
                return true;
            }
        }

        abilityOut = default;
        return false;
    }

    /// <summary>
    ///    Private method to check if the wildcard is mastery
    /// </summary>
    /// <returns></returns>
    private bool IsMastery()
    {
        return Token == "MAS";
    }

    /// <summary>
    ///    Private method to check if the wildcard is Spell DC
    /// </summary>
    /// <returns></returns>
    private bool IsDc()
    {
        return Token == "DC";
    }

    /// <summary>
    ///     Resolved this wildcard for a given character
    /// </summary>
    /// <param name="caster"> The character for which this wildcard needs to be computed </param>
    /// <returns> A score modifier that represent the resolved wildcard </returns>
    public ScoreModifier Resolve(Character caster)
    {
        if (IsAnAbility(out var ability))
        {
            return caster.AbilityScores.GetModifier(ability);
        }
        if (IsMastery())
        {
            return new ScoreModifier(caster.AbilityScores.MasteryBonus);
        }
        if (IsDc())
        {
            return new ScoreModifier(caster.Dc.GetDc(caster));
        }

        return ScoreModifier.Empty;
    }
}
