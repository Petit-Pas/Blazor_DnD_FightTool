using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Skills;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace DnDEntitiesBlazorComponents.DnDEntities.Skills;

public partial class SkillsEditableList : ComponentBase
{
    [Parameter, EditorRequired]
    public SkillCollection? Skills {
        set
        {
            if (value != null)
            {
                SkillMasteries = value.Select(x => new MasteryDto(x));
            }
        }
        get => new SkillCollection(SkillMasteries.Select(x => x.Skill));
    }

    [Parameter] public BorderRadius? BorderRadius { get; set; } = new (1, "em");


    private IEnumerable<MasteryDto> SkillMasteries { get; set; } = new List<MasteryDto>();

    [Parameter, EditorRequired]
    public AbilityScoresCollection? Abilities { get; set; }

    private class MasteryDto
    {
        public readonly Skill Skill;

        public MasteryDto(Skill skill)
        {
            Skill = skill;
        }

        public bool Expert
        {
            get => Skill.Mastery == SkillMasteryEnum.Expertise;
            set => Skill.Mastery = value ? SkillMasteryEnum.Expertise : SkillMasteryEnum.Normal;
        }

        public bool Mastered
        {
            get => Skill.Mastery == SkillMasteryEnum.Mastery;
            set => Skill.Mastery = value ? SkillMasteryEnum.Mastery : SkillMasteryEnum.Normal;
        }
    }
}