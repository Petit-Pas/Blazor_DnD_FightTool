using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.SharedComponents.Containers;

/// <summary>
///     Will keep content invisible until the parent is hovered. Parent must be configured in the isolated css here
/// </summary>
public partial class HoverableVanishingRow
{
    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }
}
