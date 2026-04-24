using DnDFightTool.Domain.CharacterSheet.Damage;

namespace DnDFightTool.Business.DnDActions.LogActions;

/// <summary>
///     Extension methods to bridge <see cref="DamageTypeEnum"/> with log formatting tags.
/// </summary>
public static class DamageTypeLogExtensions
{
    /// <summary>
    ///     Converts a <see cref="DamageTypeEnum"/> to its corresponding BBCode color tag name.
    /// </summary>
    public static string ToLogColorToken(this DamageTypeEnum damageType)
    {
        return damageType.ToString().Replace("_", "-").ToLowerInvariant();
    }
}
