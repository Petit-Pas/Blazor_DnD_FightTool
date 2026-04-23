namespace DnDFightTool.Domain.CharacterSheet.AbilityScores;

/// <summary>
///     The six core ability scores of a D&amp;D character.
/// </summary>
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
        return ability.ToString()[..3].ToUpper();
    }
    
    public readonly static AbilityEnum[] All = Enum.GetValues<AbilityEnum>();
}
