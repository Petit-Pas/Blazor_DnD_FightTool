using Characters.DamageAffinities;
using Fight.Damage;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace DnDBlazorComponents.Characters.DamageAffinities;

public partial class DamageAffinitiesEditableList : ComponentBase
{
    [Parameter]
    public DamageAffinitiesCollection? Affinities
    {
        set
        {
            if (value != null)
            {
                AffinitiesDtos = value.Select(x => new AffinityDto(x));
            }
        }
    }

    [Parameter] public BorderRadius? BorderRadius { get; set; } = new (1, "em");

    private IEnumerable<AffinityDto> AffinitiesDtos { get; set; } = new List<AffinityDto>();

    private class AffinityDto
    {
        public readonly DamageAffinity Affinity;

        public AffinityDto(DamageAffinity affinity)
        {
            Affinity = affinity;
        }

        public bool Weak
        {
            get => Affinity.Modifier == DamageFactorModifierEnum.Weak;
            set => Affinity.Modifier = value ? DamageFactorModifierEnum.Weak : Affinity.Modifier;
        }

        public bool Normal
        {
            get => Affinity.Modifier == DamageFactorModifierEnum.Normal;
            set => Affinity.Modifier = value ? DamageFactorModifierEnum.Normal : Affinity.Modifier;
        }

        public bool Resistant
        {
            get => Affinity.Modifier == DamageFactorModifierEnum.Resistant;
            set => Affinity.Modifier = value ? DamageFactorModifierEnum.Resistant : Affinity.Modifier;
        }

        public bool Immune
        {
            get => Affinity.Modifier == DamageFactorModifierEnum.Immune;
            set => Affinity.Modifier = value ? DamageFactorModifierEnum.Immune : Affinity.Modifier;
        }
    }
}