using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Skills;
using Microsoft.AspNetCore.Components;
using SharedComponents.Icons;

namespace DnDEntitiesBlazorComponents.DnDEntities.Skills;

public partial class SkillsEditorComponent
{
    [Parameter, EditorRequired]
    public Character? Character { get; set; }

    private string GetIconFor(Skill skill)
    {
        return skill.Mastery switch
        {
            SkillMasteryEnum.Normal => FontAwesomeIcons.StarEmpty,
            SkillMasteryEnum.Mastery => FontAwesomeIcons.StarHalf,
            SkillMasteryEnum.Expertise => FontAwesomeIcons.StarFull,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
