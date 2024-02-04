// ReSharper disable InconsistentNaming
namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///     An enum for the different types of damage
///     TODO Maybe at some point silver and magic should be handled differently
/// </summary>
public enum DamageTypeEnum
{
    Bludgeoning,
    Piercing,
    Slashing,
    Bludgeoning_Silver,
    Piercing_Silver,
    Slashing_Silver,
    Bludgeoning_Magic,
    Piercing_Magic,
    Slashing_Magic,
    Acid,
    Cold,
    Fire,
    Force,
    Lightning,
    Necrotic,
    Poison,
    Psychic,
    Radiant,
    Thunder
}

/// <summary>
///     Extensions methods for <see cref="DamageTypeEnum"/>
/// </summary>
public static class DamageTypeEnumExtensions
{
    /// <summary>
    ///    Returns a readable string for the enum, mainly serves for _silver and _magic.
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public static string ToReadableString(this DamageTypeEnum skill)
    {
        return skill.ToString().Replace("_", " ");
    }
}