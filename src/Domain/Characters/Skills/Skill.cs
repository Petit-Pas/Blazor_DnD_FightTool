using System.Reflection.Metadata.Ecma335;
using Characters.AbilityScores;
using Extensions;
using Fight.DiceThrows.Modifiers;

namespace Characters.Skills;

public class Skill
{
    public Skill(SkillEnum name)
    {
        Name = name;
    }

    public SkillEnum Name { get; set; }

    public SkillMasteryEnum Mastery { get; set; }

    public ScoreModifier GetModifier(AbilityScoresCollection abilityScores)
    {
        if (!Enum.IsDefined(typeof(SkillMasteryEnum), Mastery))
        {
            Console.WriteLine($"WARNING: the skill {Name} has an unknown mastery.");
            return ScoreModifier.Empty;
        }

        var attribute = Name.GetAttributes<AbilityAttribute>().SingleOrDefault();

        if (attribute == null)
        {
            Console.WriteLine($"WARNING: the skill {Name} does not have a linked attribute.");
            return ScoreModifier.Empty;
        }

        var baseScore = abilityScores.GetModifierWithoutMastery(attribute.GetAbility()).Modifier;

        return new ScoreModifier(Mastery switch
        {
            SkillMasteryEnum.Normal => baseScore,
            SkillMasteryEnum.Mastery => baseScore + abilityScores.MasteryBonus,
            SkillMasteryEnum.Expertise => baseScore + abilityScores.MasteryBonus * 2,
            _ => throw new ArgumentOutOfRangeException(nameof(Mastery), "Invalid value for SkillMasteryEnum")
        });
    }
}