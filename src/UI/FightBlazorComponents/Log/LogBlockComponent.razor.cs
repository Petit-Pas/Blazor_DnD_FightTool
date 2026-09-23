using DnDFightTool.Domain.Logs;
using DnDFightTool.UI.SharedComponents;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.FightBlazorComponents.Log;

/// <summary>
///     Renders a single <see cref="LogBlock"/> with its entries and block-level hover highlight.
/// </summary>
public partial class LogBlockComponent : StylableComponentBase
{
    /// <summary>
    ///     The log block to render.
    /// </summary>
    [Parameter]
    public LogBlock Block { get; set; } = null!;

    /// <summary>
    ///     The log service used for visibility checks.
    /// </summary>
    [Inject]
    private IDnDLogService LogService { get; set; } = null!;
}
