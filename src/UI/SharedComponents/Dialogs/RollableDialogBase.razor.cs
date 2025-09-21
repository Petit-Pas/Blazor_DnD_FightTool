using Microsoft.AspNetCore.Components;

namespace SharedComponents.Dialogs;

public partial class RollableDialogBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool CanRoll { get; set; } = true;

    [Parameter]
    public bool CanValidate { get; set; } = true;

    [Parameter]
    public EventCallback OnRollClick { get; set; }

    [Parameter]
    public EventCallback OnValidateClick { get; set; }

    protected async Task RollAsync()
    {
        if (OnRollClick.HasDelegate)
        {
            await OnRollClick.InvokeAsync();
        }
    }

    protected async Task ValidateAsync()
    {
        if (OnValidateClick.HasDelegate)
        {
            await OnValidateClick.InvokeAsync();
        }
    }
}
