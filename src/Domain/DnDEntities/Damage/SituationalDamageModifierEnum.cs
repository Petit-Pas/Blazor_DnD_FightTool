using Extensions;

namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///     An enum for modifying damage based on the situation.
///     Could be for instance what happens when you succeed on a saving throw.
/// </summary>
public enum SituationalDamageModifierEnum
{
    /// <summary>
    ///     Unknown value, will do nothing
    /// </summary>
    Unknown = 0,
    /// <summary>
    ///    No modification
    /// </summary>
    [DamageFactor(DamageFactor.DoNothingFactor)]    Normal,
    /// <summary>
    ///     Damage should be halved
    /// </summary>
    [DamageFactor(DamageFactor.HalfFactor)]         Halved,
    /// <summary>
    ///    Damage should be Canceled
    /// </summary>
    [DamageFactor(DamageFactor.NullifyFactor)]      Canceled
}

/// <summary>
///     Extensions methods for <see cref="SituationalDamageModifierEnum"/>
/// </summary>
public static class SituationalDamageModifierEnumExtensions
{
    /// <summary>
    ///    Gets the <see cref="DamageFactor"/> associated with the <see cref="SituationalDamageModifierEnum"/>
    /// </summary>
    /// <param name="damageModifier"></param>
    /// <returns></returns>
    public static DamageFactor GetFactor(this SituationalDamageModifierEnum damageModifier)
    {
        if (damageModifier != default)
        {
            var factorAttribute = damageModifier.GetAttribute<DamageFactorAttribute>();
            if (factorAttribute != default)
            {
                return factorAttribute.GetModifier();
            }
        }
        Console.WriteLine($"WARNING: Could not get the DamageFactorAttribute from a SituationalDamageModifierEnum with value: {damageModifier}.\r\nWill default to do nothing.");
        return DamageFactor.DoNothing;
    }
}
