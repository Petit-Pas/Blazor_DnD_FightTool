using System.ComponentModel;
using DnDFightTool.Domain.CharacterSheet.DamageAffinities;
using DnDFightTool.UI.SharedComponents.Icons;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.DamageAffinities;

/// <summary>
///     Class to contain UI related extension methods for <see cref="DamageAffinity" />"/>
/// </summary>
internal static class DamageAffinityExtensions
{
    public static string GetIcon(this DamageAffinity damageAffinity)
    {
        return damageAffinity.Affinity switch
        {
            DamageAffinityEnum.Weak => CustomIcons.FontAwesome.HeartBroken,
            DamageAffinityEnum.Normal => CustomIcons.FontAwesome.HeartFull,
            DamageAffinityEnum.Resistant => CustomIcons.FontAwesome.ShieldHalf,
            DamageAffinityEnum.Immune => CustomIcons.FontAwesome.ShieldFull,
            DamageAffinityEnum.Heal => CustomIcons.FontAwesome.ShieldHeart,
            _ => throw new InvalidEnumArgumentException($"{nameof(damageAffinity.Affinity)} does not have a proper icon mapped.")
        };
    }
}
