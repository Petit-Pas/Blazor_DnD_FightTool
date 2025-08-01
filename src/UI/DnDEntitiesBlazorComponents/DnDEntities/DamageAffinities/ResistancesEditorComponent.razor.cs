using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SharedComponents.Icons;

namespace DnDEntitiesBlazorComponents.DnDEntities.DamageAffinities;

public partial class ResistancesEditorComponent
{
    [Parameter, EditorRequired]
    public Character? Character { get; set; } = default!;

    private string GetIconFor(DamageAffinity damageAffinity)
    {
        return damageAffinity.Affinity switch
        {
            DamageAffinityEnum.Weak => FontAwesomeIcons.HeartBroken,
            DamageAffinityEnum.Normal => FontAwesomeIcons.HeartFull,
            DamageAffinityEnum.Resistant => FontAwesomeIcons.ShieldHalf,
            DamageAffinityEnum.Immune => FontAwesomeIcons.ShieldFull,
            DamageAffinityEnum.Heal => FontAwesomeIcons.ShieldHeart,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void HandleClick(MouseEventArgs ea)
    {
    }
}
