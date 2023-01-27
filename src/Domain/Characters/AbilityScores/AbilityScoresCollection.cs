
using Fight.DiceThrows.Modifiers;

namespace Characters.AbilityScores;

public class AbilityScoresCollection : List<AbilityScore>
{
    public AbilityScoresCollection()
    {
        AddRange(new AbilityScore[]
        {
            new (AbilityEnum.Strength, 10),
            new (AbilityEnum.Dexterity, 10),
            new (AbilityEnum.Constitution, 10),
            new (AbilityEnum.Intelligence, 10),
            new (AbilityEnum.Wisdom, 10),
            new (AbilityEnum.Charisma, 10),
        });
    }

    public ScoreModifier GetModifierWithoutMastery(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Name == name);

        if (ability == null)
        {
            // TODO warn
            return ScoreModifier.Empty;
        }

        return ability.GetModifier();
    }

    public ScoreModifier GetModifierWithMastery(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Name == name);

        if (ability == null)
        {
            // TODO warn
            return ScoreModifier.Empty;
        }

        return ability.GetModifier(MasteryBonus);
    }

    public int MasteryBonus { get; set; } = 2;
}