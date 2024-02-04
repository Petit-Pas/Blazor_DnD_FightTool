using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace SharedComponents.Overlays.OverlayableArea;

public partial class OverlayableArea
{
    private static BorderRadius _topBorderRadius = new BorderRadius(0, 0, 1, 1, "em");
    private static BorderRadius _rightBorderRadius = new BorderRadius(1, 0, 0, 1, "em");
    private static BorderRadius _bottomBorderRadius = new BorderRadius(1, 1, 0, 0, "em");

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public RenderFragment TopOverlay { get; set; }

    [Parameter]
    public RenderFragment RightOverlay { get; set; }

    [Parameter]
    public RenderFragment BottomOverlay { get; set; }
}
