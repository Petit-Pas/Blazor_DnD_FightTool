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
            SkillMasteryEnum.Normal => FontAwesomeIcons.EmptyStar,
            SkillMasteryEnum.Mastery => FontAwesomeIcons.HalfStar,
            SkillMasteryEnum.Expertise => FontAwesomeIcons.FullStar,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
