using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;

namespace DnDFightTool.Domain.DnDEntities.Skills;

/// <summary>
///     A collection for skills
/// </summary>
public class SkillCollection : Dictionary<SkillEnum, Skill>
{
    /// <summary>
    ///     Empty Ctor, should only be used by serializers
    /// </summary>
    [Obsolete("Should only be used by serializers")]
    public SkillCollection() : this(false)
    {
    }

    /// <summary>
    ///     Ctor that receives the skill directly
    /// </summary>
    /// <param name="skills"></param>
    public SkillCollection(IEnumerable<Skill> skills)
    {
        foreach (var skill in skills)
        {
            Add(skill);
        }
    }

    /// <summary>
    ///     Ctor that allows to setup the default skills that exist in D&D
    /// </summary>
    /// <param name="withDefaults"></param>
    public SkillCollection(bool withDefaults = false)
    {
        if (withDefaults)
        {
            AddRange(new Skill[]
            {
                new(SkillEnum.Acrobatic),
                new(SkillEnum.Animal_Handling),
                new(SkillEnum.Arcana),
                new(SkillEnum.Athletics),
                new(SkillEnum.Deception),
                new(SkillEnum.History),
                new(SkillEnum.Insight),
                new(SkillEnum.Intimidation),
                new(SkillEnum.Investigation),
                new(SkillEnum.Medicine),
                new(SkillEnum.Nature),
                new(SkillEnum.Perception),
                new(SkillEnum.Performance),
                new(SkillEnum.Persuasion),
                new(SkillEnum.Religion),
                new(SkillEnum.Sleight_Of_Hand),
                new(SkillEnum.Stealth),
                new(SkillEnum.Survival),
            });
        }
    }

    /// <summary>
    ///     Helper method to add in the dictionary without having to take care about the key
    /// </summary>
    /// <param name="skill"></param>
    private void Add(Skill skill)
    {
        this.Add(skill.Name, skill);
    }

    /// <summary>
    ///     Helper method to bulk insert in the dictionary without having to take care about the key
    /// </summary>
    /// <param name="skills"></param>
    private void AddRange(IEnumerable<Skill> skills)
    {
        foreach (var skill in skills)
        {
            Add(skill);
        }
    }

    /// <summary>
    ///     Gets the modifier a character should apply to a skill based on its ability scores & level of expertise.
    /// </summary>
    /// <param name="skillName"></param>
    /// <param name="abilityScores"></param>
    /// <returns></returns>
    public ScoreModifier GetModifier(SkillEnum skillName, AbilityScoresCollection abilityScores)
    {
        if (this.TryGetValue(skillName, out var skill))
        {
            return skill.GetModifier(abilityScores);
        }
        // TODO warn
        return ScoreModifier.Empty;
    }
}