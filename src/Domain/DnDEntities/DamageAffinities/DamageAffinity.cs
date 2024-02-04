using DnDFightTool.Domain.DnDEntities.Damage;
using Extensions;

namespace DnDFightTool.Domain.DnDEntities.DamageAffinities;

/// <summary>
///     Represents the affinity of a damage type for a character
/// </summary>
public class DamageAffinity
{
    /// <summary>
    ///     Empty ctor, should only be used by serializers
    /// </summary>
    [Obsolete("Should only be used by serializers")]
    public DamageAffinity()
    {
    }

    /// <summary>
    ///     Proper ctor
    /// </summary>
    /// <param name="type"></param>
    /// <param name="modifier"></param>
    public DamageAffinity(DamageTypeEnum type, DamageAffinityEnum modifier = DamageAffinityEnum.Normal)
    {
        Type = type;
        Affinity = modifier;
    }

    /// <summary>
    ///    A way to retrieve the Damage factor according to the affinity to this type of damage.
    /// </summary>
    /// <returns></returns>
    public DamageFactor GetDamageFactor()
    {
        var modifier = Affinity.GetAttribute<DamageFactorAttribute>();
        if (modifier == null)
        {
            Console.WriteLine($"WARNING: the skill {Type} has no DamageAffinitFactory attribute.");
            return DamageFactor.DoNothing;
        }

        return new DamageFactor(modifier.GetModifier());
    }

    /// <summary>
    ///     The type of damage
    /// </summary>
    public DamageTypeEnum Type { get; set; }

    /// <summary>
    ///     The affinity of the character for this type of damage
    /// </summary>
    public DamageAffinityEnum Affinity { get; set; }
}