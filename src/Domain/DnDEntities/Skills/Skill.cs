using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Characters;
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
    /// <param name="skillName"></param>
    public Skill(SkillEnum skillName)
    {
        Name = skillName;
        Ability = skillName.GetAttribute<AbilityAttribute>()!.Ability;
        DefaultAbility = Ability;
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
    ///     The ability to use to do the skill check.
    ///     Default is set by <see cref="AbilityAttribute"/> on <see cref="SkillEnum"/>
    /// </summary>
    public AbilityEnum Ability { get; set; }

    /// <summary>
    ///     The ability used by default to do the skill check
    /// </summary>
    public AbilityEnum DefaultAbility { get; private init; }

    public void IncreaseMastery()
    {
        Mastery = Mastery switch {
            SkillMasteryEnum.Normal => SkillMasteryEnum.Mastery,
            SkillMasteryEnum.Mastery => SkillMasteryEnum.Expertise,
            SkillMasteryEnum.Expertise => SkillMasteryEnum.Expertise,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void DecreaseMastery()
    {
        Mastery = Mastery switch
        {
            SkillMasteryEnum.Normal => SkillMasteryEnum.Normal,
            SkillMasteryEnum.Mastery => SkillMasteryEnum.Normal,
            SkillMasteryEnum.Expertise => SkillMasteryEnum.Mastery,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    ///     Shorter way to call <see cref="GetModifier(AbilityScoresCollection)"/>
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public ScoreModifier GetModifier(Character character)
    {
        return GetModifier(character.AbilityScores);
    }

    /// <summary>
    ///     Gets a score modifier to a skill check based on the ability scores of the character as well as its mastery of the skill
    /// </summary>
    /// <param name="abilityScores"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ScoreModifier GetModifier(AbilityScoresCollection abilityScores)
    {
        var baseScore = abilityScores.GetModifier(Ability);

        return Mastery switch
        {
            SkillMasteryEnum.Normal => baseScore,
            SkillMasteryEnum.Mastery => baseScore + abilityScores.MasteryBonus,
            SkillMasteryEnum.Expertise => baseScore + abilityScores.MasteryBonus * 2,
#pragma warning disable IDE0079
#pragma warning disable CA2208
            _ => throw new ArgumentOutOfRangeException(nameof(Mastery), "Invalid value for SkillMasteryEnum")
#pragma warning restore CA2208
#pragma warning restore IDE0079
        };
    }
}