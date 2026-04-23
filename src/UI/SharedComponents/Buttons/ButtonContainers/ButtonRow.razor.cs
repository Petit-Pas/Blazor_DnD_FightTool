using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.SharedComponents.Buttons.ButtonContainers;

public partial class ButtonRow
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
