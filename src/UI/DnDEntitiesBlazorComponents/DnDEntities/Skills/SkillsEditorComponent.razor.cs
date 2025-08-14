using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Skills;
using Microsoft.AspNetCore.Components;
using SharedComponents;
using SharedComponents.Icons;

namespace DnDEntitiesBlazorComponents.DnDEntities.Skills;

public partial class SkillsEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public Character? Character { get; set; }

    private string GetIconFor(Skill skill)
    {
        return skill.Mastery switch
        {
            SkillMasteryEnum.Normal => CustomIcons.FontAwesome.StarEmpty,
            SkillMasteryEnum.Mastery => CustomIcons.FontAwesome.StarHalf,
            SkillMasteryEnum.Expertise => CustomIcons.FontAwesome.StarFull,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
