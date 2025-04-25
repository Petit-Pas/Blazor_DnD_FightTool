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
            AddRange(DamageTypeEnumExtensions.All.Select(x => new DamageAffinity(x)));
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