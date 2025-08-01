using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Damage;

namespace DnDFightTool.Domain.DnDEntities.DamageAffinities;

// TODO unit tests

/// <summary>
///     A collection of damage affinities
/// </summary>
public class DamageAffinitiesCollection : Dictionary<DamageTypeEnum, DamageAffinity>
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
    ///     Helper method to add in the dictionary without having to take care about the key
    /// </summary>
    /// <param name="affinity"></param>
    private void Add(DamageAffinity affinity)
    {
        Add(affinity.DamageType, affinity);
    }

    /// <summary>
    ///     Helper method to bulk insert in the dictionary without having to take care about the key
    /// </summary>
    /// <param name="skills"></param>
    private void AddRange(IEnumerable<DamageAffinity> damageAffinities)
    {
        foreach (var damageAffinity in damageAffinities)
        {
            Add(damageAffinity);
        }
    }

    /// <summary>
    ///     Get the damage affinity object for a specific damage type
    /// </summary>
    /// <param name="damageType"></param>
    /// <returns></returns>
    private DamageAffinity? GetDamageAffinityFor(DamageTypeEnum damageType)
    {
        if (TryGetValue(damageType, out var damageAffinity))
        {
            return damageAffinity;
        }
        // Warn?
        return new DamageAffinity(damageType);
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