using DnDEntities.Damage;

namespace DnDEntities.DamageAffinities;

public class DamageAffinitiesCollection : List<DamageAffinity>
{
    public DamageAffinitiesCollection() : this(false)
    {
    }

    public DamageAffinitiesCollection(bool withDefaults = false)
    {
        if (withDefaults)
        {
            AddRange(new[]
            {
                new DamageAffinity(DamageTypeEnum.Bludgeoning),
                new DamageAffinity(DamageTypeEnum.Piercing),
                new DamageAffinity(DamageTypeEnum.Slashing),
                new DamageAffinity(DamageTypeEnum.Bludgeoning_Silver),
                new DamageAffinity(DamageTypeEnum.Piercing_Silver),
                new DamageAffinity(DamageTypeEnum.Slashing_Silver),
                new DamageAffinity(DamageTypeEnum.Bludgeoning_Magic),
                new DamageAffinity(DamageTypeEnum.Piercing_Magic),
                new DamageAffinity(DamageTypeEnum.Slashing_Magic),
                new DamageAffinity(DamageTypeEnum.Acid),
                new DamageAffinity(DamageTypeEnum.Cold),
                new DamageAffinity(DamageTypeEnum.Fire),
                new DamageAffinity(DamageTypeEnum.Force),
                new DamageAffinity(DamageTypeEnum.Lightning),
                new DamageAffinity(DamageTypeEnum.Necrotic),
                new DamageAffinity(DamageTypeEnum.Poison),
                new DamageAffinity(DamageTypeEnum.Psychic),
                new DamageAffinity(DamageTypeEnum.Radiant),
                new DamageAffinity(DamageTypeEnum.Thunder),
            });
        }
    }

    private DamageAffinity? GetDamageAffinityFor(DamageTypeEnum damageType)
    {
        var damageAffinity = this.SingleOrDefault(x => x.Type == damageType);
        if (damageAffinity == null)
        {
            // TODO warn
            return null;
        }
        return damageAffinity;
    }

    public DamageAffinityEnum GetAffinityFor(DamageTypeEnum damageType)
    {
        var damageAffinity = GetDamageAffinityFor(damageType);

        return damageAffinity?.Affinity ?? DamageAffinityEnum.Normal;
    }

    public DamageFactor GetDamageFactorFor(DamageTypeEnum damageType)
    {
        var damageAffinity = GetDamageAffinityFor(damageType);

        return damageAffinity?.GetDamageFactor() ?? DamageFactor.None;
    }
}