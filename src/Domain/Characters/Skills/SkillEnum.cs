using Characters.AbilityScores;

namespace Characters.Skills;

public enum SkillEnum
{
    [Ability(AbilityEnum.Dexterity)] Acrobatic,
    [Ability(AbilityEnum.Wisdom)] Animal_Handling,
    [Ability(AbilityEnum.Intelligence)] Arcana,
    [Ability(AbilityEnum.Strength)] Athletics,
    [Ability(AbilityEnum.Charisma)] Deception,
    [Ability(AbilityEnum.Intelligence)] History,
    [Ability(AbilityEnum.Wisdom)] Insight,
    [Ability(AbilityEnum.Charisma)] Intimidation,
    [Ability(AbilityEnum.Intelligence)] Investigation,
    [Ability(AbilityEnum.Wisdom)] Medicine,
    [Ability(AbilityEnum.Intelligence)] Nature,
    [Ability(AbilityEnum.Wisdom)] Perception,
    [Ability(AbilityEnum.Charisma)] Performance,
    [Ability(AbilityEnum.Charisma)] Persuasion,
    [Ability(AbilityEnum.Intelligence)] Religion,
    [Ability(AbilityEnum.Dexterity)] Sleight_Of_Hand,
    [Ability(AbilityEnum.Dexterity)] Stealth,
    [Ability(AbilityEnum.Wisdom)] Survival,
}

public static class SkillEnumExtensions
{
    public static string ToReadableString(this SkillEnum skill)
    {
        return skill.ToString().Replace("_", " ");
    }
}