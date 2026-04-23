using System.Diagnostics.CodeAnalysis;

namespace DnDFightTool.Domain.CharacterSheet.AbilityScores;

[AttributeUsage(AttributeTargets.Field)]
public class AbilityAttribute : Attribute
{
    public required AbilityEnum Ability { get; set; }

    [SetsRequiredMembers]
    public AbilityAttribute(AbilityEnum ability)
    {
        Ability = ability;
    }
}
