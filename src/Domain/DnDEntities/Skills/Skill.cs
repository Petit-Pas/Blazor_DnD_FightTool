using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;
using Extensions;

namespace DnDFightTool.Domain.DnDEntities.Skills;

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
        var skillAttribute = Name.GetAttribute<AbilityAttribute>();

        if (skillAttribute == null)
        {
            Console.WriteLine($"WARNING: the skill {Name} does not have a linked attribute.");
            return ScoreModifier.Empty;
        }

        var baseScore = abilityScores.GetModifierWithoutMastery(skillAttribute.GetAbility()).Modifier;

        return new ScoreModifier(Mastery switch
        {
            SkillMasteryEnum.Normal => baseScore,
            SkillMasteryEnum.Mastery => baseScore + abilityScores.MasteryBonus,
            SkillMasteryEnum.Expertise => baseScore + abilityScores.MasteryBonus * 2,
            _ => throw new ArgumentOutOfRangeException(nameof(Mastery), "Invalid value for SkillMasteryEnum")
        });
    }
}