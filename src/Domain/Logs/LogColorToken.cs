namespace DnDFightTool.Domain.Logs;

/// <summary>
///     Supported semantic color tokens for log entry formatting.
///     Each value maps to a CSS custom property: <c>--dnd-color-{kebab-case-name}</c>.
/// </summary>
public enum LogColorToken
{
    Bludgeoning,
    Piercing,
    Slashing,
    BludgeoningSilver,
    PiercingSilver,
    SlashingSilver,
    BludgeoningMagic,
    PiercingMagic,
    SlashingMagic,
    Acid,
    Cold,
    Fire,
    Force,
    Lightning,
    Necrotic,
    Poison,
    Psychic,
    Radiant,
    Thunder,
    Heal
}

/// <summary>
///     Extension methods for <see cref="LogColorToken"/>.
/// </summary>
public static class LogColorTokenExtensions
{
    /// <summary>
    ///     All values of <see cref="LogColorToken"/>.
    /// </summary>
    public static IReadOnlyList<LogColorToken> All { get; } = Enum.GetValues<LogColorToken>();

    /// <summary>
    ///     Converts the token to its kebab-case CSS variable name segment (e.g., <c>BludgeoningSilver</c> → <c>bludgeoning-silver</c>).
    /// </summary>
    public static string ToKebabCase(this LogColorToken token)
    {
        var name = token.ToString();
        var builder = new System.Text.StringBuilder(name.Length + 4);

        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (i > 0 && char.IsUpper(c))
            {
                builder.Append('-');
            }
            builder.Append(char.ToLowerInvariant(c));
        }

        return builder.ToString();
    }
}
