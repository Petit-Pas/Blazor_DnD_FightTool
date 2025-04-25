using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace DnDEntitiesBlazorComponents.DnDEntities.DamageAffinities;

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

    [Parameter] 
    public BorderRadius? BorderRadius { get; set; } = new (1, "em");

    private IEnumerable<AffinityDto> AffinitiesDtos { get; set; } = [];

    private class AffinityDto
    {
        public readonly DamageAffinity Affinity;

        public AffinityDto(DamageAffinity affinity)
        {
            Affinity = affinity;
        }

        public bool Weak
        {
            get => Affinity.Affinity == DamageAffinityEnum.Weak;
            set => Affinity.Affinity = value ? DamageAffinityEnum.Weak : Affinity.Affinity;
        }

        public bool Normal
        {
            get => Affinity.Affinity == DamageAffinityEnum.Normal;
            set => Affinity.Affinity = value ? DamageAffinityEnum.Normal : Affinity.Affinity;
        }

        public bool Resistant
        {
            get => Affinity.Affinity == DamageAffinityEnum.Resistant;
            set => Affinity.Affinity = value ? DamageAffinityEnum.Resistant : Affinity.Affinity;
        }

        public bool Immune
        {
            get => Affinity.Affinity == DamageAffinityEnum.Immune;
            set => Affinity.Affinity = value ? DamageAffinityEnum.Immune : Affinity.Affinity;
        }
    }
}