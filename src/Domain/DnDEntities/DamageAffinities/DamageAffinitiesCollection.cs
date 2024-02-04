using DnDFightTool.Domain.DnDEntities.Damage;

namespace DnDFightTool.Domain.DnDEntities.DamageAffinities;

// TODO unit tests

/// <summary>
///     A collection of damage affinities
/// </summary>
public class DamageAffinitiesCollection : List<DamageAffinity>
{
    /// <summary>
    ///     default ctor, Will not populate the collection with default values
    ///     mostly used for serialization
    /// </summary>
    public DamageAffinitiesCollection() : this(false)
    {
    }

    /// <summary>
    ///     ctor that allows to specify that the collection should be populated with default values
    /// </summary>
    /// <param name="withDefaults"></param>
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

    /// <summary>
    ///     Get the damage affinity object for a specific damage type
    /// </summary>
    /// <param name="damageType"></param>
    /// <returns></returns>
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

    /// <summary>
    ///     Get the damage factory of a specific damage type
    /// </summary>
    /// <param name="damageType"></param>
    /// <returns></returns>
    public DamageFactor GetDamageFactorFor(DamageTypeEnum damageType)
    {
        var damageAffinity = GetDamageAffinityFor(damageType);

        return damageAffinity?.GetDamageFactor() ?? DamageFactor.DoNothing;
    }
}