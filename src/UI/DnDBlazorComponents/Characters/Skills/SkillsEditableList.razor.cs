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
        public Skill Skill;

        public MasteryDto(Skill skill)
        {
            Skill = skill;
            switch (skill.Mastery)
            {
                case SkillMasteryEnum.Normal:
                    break;
                case SkillMasteryEnum.Mastery:
                    _mastered = true;
                    break;
                case SkillMasteryEnum.Expertise:
                    _expert = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CheckReset()
        {
            if (Mastered == false && Expert == false)
            {
                Skill.Mastery = SkillMasteryEnum.Normal;
            }
        }

        public bool Expert
        {
            get => _expert;
            set
            {
                if (value == true)
                {
                    _mastered = false;
                    Skill.Mastery = SkillMasteryEnum.Expertise;
                }
                _expert = value;
                CheckReset();
            }
        }
        private bool _expert = false;

        public bool Mastered
        {
            get => _mastered;
            set
            {
                if (value == true)
                {
                    _expert = false;
                    Skill.Mastery = SkillMasteryEnum.Mastery;
                }
                _mastered = value;
                CheckReset();
            }
        }
        private bool _mastered = false;

    }
}