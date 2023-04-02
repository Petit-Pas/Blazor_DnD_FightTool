using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace SharedComponents.Menus.MenuBlades;

public partial class MenuBlade
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter] 
    public ElementRelativePosition Position { get; set; } = ElementRelativePosition.Top;

    // UI Methods
    private string PositionClass => Position switch
    {
        ElementRelativePosition.Top => "top",
        ElementRelativePosition.Bottom => "bottom",
        _ => throw new NotImplementedException($"MenuBlade does not support {Position} at the moment."),
    };
}
