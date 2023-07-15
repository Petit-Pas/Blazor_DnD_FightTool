namespace DnDFightTool.Domain.DnDEntities.AbilityScores;

public enum AbilityEnum
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma,
}

public static class AbilityEnumExtensions
{
    public static string ShortName(this AbilityEnum ability)
    {
        return ability.ToString().Substring(0, 3).ToUpper();
    }
}
