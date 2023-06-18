using DnDEntities.Damage;
using Extensions;

namespace DnDEntities.DamageAffinities;

public class DamageAffinity
{
    public DamageAffinity()
    {
    }

    public DamageAffinity(DamageTypeEnum type, DamageAffinityEnum modifier = DamageAffinityEnum.Normal)
    {
        Type = type;
        Affinity = modifier;
    }

    public DamageFactor GetDamageFactor()
    {
        var modifier = Affinity.GetAttribute<DamageFactorAttribute>();
        if (modifier == null)
        {
            Console.WriteLine($"WARNING: the skill {Type} has no DamageAffinitFactory attribute.");
            return DamageFactor.None;
        }

        return new DamageFactor(modifier.GetModifier());
    }

    public DamageTypeEnum Type { get; set; }
    public DamageAffinityEnum Affinity { get; set; }
}