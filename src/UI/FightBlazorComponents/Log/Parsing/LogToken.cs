using DnDFightTool.Domain.Logs;

namespace DnDFightTool.UI.FightBlazorComponents.Log.Parsing;

/// <summary>
///     Represents a parsed token from BBCode-like log entry content.
/// </summary>
public abstract record LogToken
{
    /// <summary>
    ///     A plain text segment.
    /// </summary>
    public sealed record TextToken(string Text) : LogToken;

    /// <summary>
    ///     Opens a bold span.
    /// </summary>
    public sealed record BoldStart : LogToken;

    /// <summary>
    ///     Closes a bold span.
    /// </summary>
    public sealed record BoldEnd : LogToken;

    /// <summary>
    ///     Opens a colored span with a validated semantic color token.
    /// </summary>
    public sealed record ColorStart(LogColorToken Token) : LogToken;

    /// <summary>
    ///     Closes a colored span.
    /// </summary>
    public sealed record ColorEnd : LogToken;

    /// <summary>
    ///     Opens a hover span with tooltip text.
    /// </summary>
    public sealed record HoverStart(string TooltipText) : LogToken;

    /// <summary>
    ///     Closes a hover span.
    /// </summary>
    public sealed record HoverEnd : LogToken;
}
