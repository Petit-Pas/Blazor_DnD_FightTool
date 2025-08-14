using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SharedComponents;
using SharedComponents.Icons;

namespace DnDEntitiesBlazorComponents.DnDEntities.DamageAffinities;

public partial class ResistancesEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public Character? Character { get; set; } = default!;

    private string GetIconFor(DamageAffinity damageAffinity)
    {
        return damageAffinity.Affinity switch
        {
            DamageAffinityEnum.Weak => CustomIcons.FontAwesome.HeartBroken,
            DamageAffinityEnum.Normal => CustomIcons.FontAwesome.HeartFull,
            DamageAffinityEnum.Resistant => CustomIcons.FontAwesome.ShieldHalf,
            DamageAffinityEnum.Immune => CustomIcons.FontAwesome.ShieldFull,
            DamageAffinityEnum.Heal => CustomIcons.FontAwesome.ShieldHeart,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void HandleClick(MouseEventArgs ea)
    {
    }
}
