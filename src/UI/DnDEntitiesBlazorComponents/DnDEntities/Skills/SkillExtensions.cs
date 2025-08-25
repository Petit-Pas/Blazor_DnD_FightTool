using System.ComponentModel;
using DnDFightTool.Domain.DnDEntities.Skills;
using SharedComponents.Icons;

namespace DnDEntitiesBlazorComponents.DnDEntities.Skills;

internal static class SkillExtensions
{
    public static string GetIcon(this Skill skill)
    {
        return skill.Mastery switch
        {
            SkillMasteryEnum.Normal => CustomIcons.FontAwesome.StarEmpty,
            SkillMasteryEnum.Mastery => CustomIcons.FontAwesome.StarHalf,
            SkillMasteryEnum.Expertise => CustomIcons.FontAwesome.StarFull,
            _ => throw new InvalidEnumArgumentException($"{nameof(skill.Mastery)} does not have a proper icon mapped.")
        };
    }
}
