using Microsoft.AspNetCore.Components;

namespace SharedComponents.Buttons.ButtonContainers;

public partial class ButtonRow
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
