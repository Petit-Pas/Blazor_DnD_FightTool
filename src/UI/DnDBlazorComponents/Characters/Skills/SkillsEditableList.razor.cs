using Characters.AbilityScores;
using Characters.Skills;
using Microsoft.AspNetCore.Components;

namespace DnDBlazorComponents.Characters.Skills;

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
    }

    private IEnumerable<MasteryDto> SkillMasteries { get; set; } = new List<MasteryDto>();

    [Parameter, EditorRequired]
    public AbilityScoresCollection Abilities { get; set; }

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