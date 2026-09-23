using System.Net;
using System.Text;
using DnDFightTool.Domain.Logs;
using DnDFightTool.UI.FightBlazorComponents.Log.Parsing;
using DnDFightTool.UI.SharedComponents;
using Microsoft.AspNetCore.Components;


namespace DnDFightTool.UI.FightBlazorComponents.Log;

/// <summary>
///     Renders a single <see cref="LogEntry"/> with parsed BBCode-like formatting.
/// </summary>
public partial class LogEntryComponent : StylableComponentBase
{
    /// <summary>
    ///     The log entry to render.
    /// </summary>
    [Parameter]
    public LogEntry Entry { get; set; } = null!;

    private List<LogSegment> BuildSegments()
    {
        var tokens = LogTokenParser.Parse(Entry.Content);
        var segments = new List<LogSegment>();
        var sb = new StringBuilder();
        string? activeTooltip = null;

        foreach (var token in tokens)
        {
            switch (token)
            {
                case LogToken.HoverStart hover:
                    if (sb.Length > 0)
                    {
                        segments.Add(new PlainSegment(sb.ToString()));
                        sb.Clear();
                    }
                    activeTooltip = hover.TooltipText;
                    break;
                case LogToken.HoverEnd:
                    segments.Add(new HoverSegment(sb.ToString(), activeTooltip ?? string.Empty));
                    sb.Clear();
                    activeTooltip = null;
                    break;
                case LogToken.TextToken text:
                    sb.Append(WebUtility.HtmlEncode(text.Text));
                    break;
                case LogToken.BoldStart:
                    sb.Append("<strong>");
                    break;
                case LogToken.BoldEnd:
                    sb.Append("</strong>");
                    break;
                case LogToken.ColorStart color:
                    sb.Append($"<span style=\"color: var(--dnd-color-{color.Token.ToKebabCase()}, inherit)\">");
                    break;
                case LogToken.ColorEnd:
                    sb.Append("</span>");
                    break;
            }
        }

        if (sb.Length > 0)
        {
            segments.Add(new PlainSegment(sb.ToString()));
        }

        return segments;
    }

    private abstract record LogSegment;
    private record PlainSegment(string Html) : LogSegment;
    private record HoverSegment(string Html, string TooltipText) : LogSegment;
}
