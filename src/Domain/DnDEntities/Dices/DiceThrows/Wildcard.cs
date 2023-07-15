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

    private bool IsAnAbility(out AbilityEnum abilityOut)
    {
        foreach (AbilityEnum ability in Enum.GetValues(typeof(AbilityEnum)))
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

    private bool IsMastery()
    {
        return Token == "MAS";
    }

    public ScoreModifier Resolve(Character caster)
    {
        if (IsAnAbility(out var ability))
        {
            return caster.AbilityScores.GetModifierWithoutMastery(ability);
        }
        if (IsMastery())
        {
            return new ScoreModifier(caster.AbilityScores.MasteryBonus);
        }

        return ScoreModifier.Empty;
    }
}
