using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;
using Extensions;

namespace DnDFightTool.Domain.DnDEntities.Skills;

/// <summary>
///     Represent a D&D skill, such as Acrobatics or Persuasion
/// </summary>
public class Skill
{
    /// <summary>
    ///     Ctor with the name of the skill
    /// </summary>
    /// <param name="name"></param>
    public Skill(SkillEnum name)
    {
        Name = name;
    }

    /// <summary>
    ///     Name of the skill
    /// </summary>
    public SkillEnum Name { get; set; }

    /// <summary>
    ///     How proficient the character is with this skill
    ///     Can be Normal, Mastery or Expertise
    /// </summary>
    public SkillMasteryEnum Mastery { get; set; }

    /// <summary>
    ///     Gets a score modifier to a skill check based on the ability scores of the character as well as its mastery of the skill
    /// </summary>
    /// <param name="abilityScores"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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