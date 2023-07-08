using DnDEntities.AbilityScores;
using DnDEntities.Dices.Modifiers;

namespace DnDEntities.Skills;

public class SkillCollection : List<Skill>
{
    public SkillCollection() : this(false)
    {
    }

    public SkillCollection(IEnumerable<Skill> skills)
    {
        foreach (var skill in skills)
        {
            Add(skill);
        }
    }

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

    public ScoreModifier GetModifier(SkillEnum skillName, AbilityScoresCollection abilityScores)
    {
        var skill = this.SingleOrDefault(x => x.Name == skillName);

        if (skill == null)
        {
            // TODO warn
            return ScoreModifier.Empty;
        }

        return skill.GetModifier(abilityScores);
    }
}