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
        DamageType = type;
        Affinity = modifier;
    }

    /// <summary>
    ///     The type of damage
    /// </summary>
    public DamageTypeEnum DamageType { get; set; }

    /// <summary>
    ///     The affinity of the character for this type of damage
    /// </summary>
    public DamageAffinityEnum Affinity { get; set; }

    /// <summary>
    ///    A way to retrieve the Damage factor according to the affinity to this type of damage.
    /// </summary>
    /// <returns></returns>
    public DamageFactor GetDamageFactor()
    {
        var modifier = Affinity.GetAttribute<DamageFactorAttribute>();
        if (modifier == null)
        {
            Console.WriteLine($"WARNING: the skill {DamageType} has no DamageAffinitFactory attribute.");
            return DamageFactor.DoNothing;
        }

        return new DamageFactor(modifier.GetModifier());
    }


    public void IncreaseAffinity()
    {
        Affinity = Affinity switch
        {
            DamageAffinityEnum.Weak => DamageAffinityEnum.Normal,
            DamageAffinityEnum.Normal => DamageAffinityEnum.Resistant,
            DamageAffinityEnum.Resistant => DamageAffinityEnum.Immune,
            DamageAffinityEnum.Immune => DamageAffinityEnum.Heal,
            DamageAffinityEnum.Heal => DamageAffinityEnum.Heal,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void DecreaseAffinity()
    {
        Affinity = Affinity switch
        {
            DamageAffinityEnum.Weak => DamageAffinityEnum.Weak,
            DamageAffinityEnum.Normal => DamageAffinityEnum.Weak,
            DamageAffinityEnum.Resistant => DamageAffinityEnum.Normal,
            DamageAffinityEnum.Immune => DamageAffinityEnum.Resistant,
            DamageAffinityEnum.Heal => DamageAffinityEnum.Immune,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}