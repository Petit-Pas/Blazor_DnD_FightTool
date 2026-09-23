using DnDFightTool.Domain.Logs;

namespace DnDFightTool.UI.FightBlazorComponents.Log.Parsing;

/// <summary>
///     Single-pass tokenizer that converts BBCode-like log content into <see cref="LogToken"/> sequences.
///     Supports <c>[b]</c>, <c>[/b]</c>, <c>[c:token]</c>, <c>[/c]</c>, <c>[hover:text]</c>, <c>[/hover]</c>.
///     Unknown or malformed tags are emitted as literal text.
/// </summary>
public static class LogTokenParser
{
    private readonly static Dictionary<string, LogColorToken> _byKebabName = LogColorTokenExtensions.All.ToDictionary(t => t.ToKebabCase(), t => t);
    
    /// <summary>
    ///     Parses the given content string into a list of tokens.
    /// </summary>
    /// <param name="content">The BBCode-like formatted string.</param>
    /// <returns>An ordered list of tokens.</returns>
    public static IReadOnlyList<LogToken> Parse(string content)
    {
        var tokens = new List<LogToken>();

        if (string.IsNullOrEmpty(content))
        {
            return tokens;
        }

        var i = 0;
        var textStart = 0;
        var insideHover = false;

        while (i < content.Length)
        {
            if (content[i] == '[')
            {
                var closeBracket = content.IndexOf(']', i + 1);
                if (closeBracket < 0)
                {
                    // No closing bracket — rest is plain text
                    break;
                }

                var tagContent = content.AsSpan(i + 1, closeBracket - i - 1);

                var token = TryParseTag(tagContent, insideHover);
                if (token is not null)
                {
                    // Flush preceding text, since the main loop iterates until it faces a token, any iterated since textStart is guaranteed to be plain text.
                    if (i > textStart)
                    {
                        tokens.Add(new LogToken.TextToken(content[textStart..i]));
                    }

                    if (token is LogToken.HoverStart)
                    {
                        insideHover = true;
                    }
                    else if (token is LogToken.HoverEnd)
                    {
                        insideHover = false;
                    }

                    tokens.Add(token);
                    i = closeBracket + 1;
                    textStart = i;
                }
                else
                {
                    // Unknown tag — treat '[' as literal and move on
                    i++;
                }
            }
            else
            {
                i++;
            }
        }

        // Flush remaining text
        if (textStart < content.Length)
        {
            tokens.Add(new LogToken.TextToken(content[textStart..]));
        }

        return tokens;
    }

    private static LogToken? TryParseTag(ReadOnlySpan<char> tagContent, bool insideHover)
    {
        if (tagContent.Equals("b", StringComparison.OrdinalIgnoreCase))
        {
            return new LogToken.BoldStart();
        }

        if (tagContent.Equals("/b", StringComparison.OrdinalIgnoreCase))
        {
            return new LogToken.BoldEnd();
        }

        if (tagContent.Equals("/c", StringComparison.OrdinalIgnoreCase))
        {
            return new LogToken.ColorEnd();
        }

        if (tagContent.Equals("/hover", StringComparison.OrdinalIgnoreCase))
        {
            return new LogToken.HoverEnd();
        }

        if (tagContent.StartsWith("c:", StringComparison.OrdinalIgnoreCase))
        {
            var tokenName = tagContent[2..];
            if (TryParseColorToken(tokenName, out var colorToken))
            {
                return new LogToken.ColorStart(colorToken);
            }

            // Unknown color token — return null so the entire tag is treated as literal text
            return null;
        }

        if (tagContent.StartsWith("hover:", StringComparison.OrdinalIgnoreCase))
        {
            if (insideHover)
            {
                // Nested hover tags are not supported — emit as literal text
                return null;
            }

            var tooltipText = tagContent[6..].ToString();
            return new LogToken.HoverStart(tooltipText);
        }

        return null;
    }

    private static bool TryParseColorToken(ReadOnlySpan<char> name, out LogColorToken result)
    {
        return _byKebabName.TryGetValue(name.ToString(), out result);
    }
}
