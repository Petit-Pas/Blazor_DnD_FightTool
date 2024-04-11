using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs.Buttons;

public partial class DeleteButton
{
    [Parameter]
    public virtual EventCallback OnClick { get; set; }

    [Parameter]
    public virtual int SizeInPx { get; set; } = 28;

    private async Task NotifyClick()
    {
        await OnClick.InvokeAsync();
    }
}
