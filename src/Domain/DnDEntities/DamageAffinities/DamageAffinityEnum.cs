using DnDFightTool.Domain.DnDEntities.Damage;

namespace DnDFightTool.Domain.DnDEntities.DamageAffinities;

/// <summary>
///     Enum representing the affinity of a creature to a specific damage type
/// </summary>
public enum DamageAffinityEnum
{
    /// <summary>
    ///     Default value, should never be used
    /// </summary>
    [DamageFactor(1)]   Unknown = 0,
    /// <summary>
    ///     creature is weak to a given damage type
    /// </summary>
    [DamageFactor(2)]   Weak,
    /// <summary>
    ///     creature interacts normally with a given damage type
    /// </summary>
    [DamageFactor(1)]   Normal,
    /// <summary>
    ///     creature is resistant to a given damage type
    /// </summary>
    [DamageFactor(0.5)] Resistant,
    /// <summary>
    ///    creature is immune to a given damage type
    /// </summary>
    [DamageFactor(0)]   Immune
}