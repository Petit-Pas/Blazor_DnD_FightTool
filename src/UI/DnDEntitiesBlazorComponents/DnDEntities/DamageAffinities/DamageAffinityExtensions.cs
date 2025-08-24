using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using SharedComponents.Icons;

namespace DnDEntitiesBlazorComponents.DnDEntities.DamageAffinities;

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
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
