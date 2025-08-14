using Microsoft.AspNetCore.Components;

namespace SharedComponents.Inputs.Buttons;

public partial class CancelSaveButtonRow : StylableComponentBase
{
    [Parameter]
    public EventCallback CancelClicked { get; set; }

    [Parameter]
    public EventCallback SaveClicked { get; set; }

    private async Task Cancel()
    {
        await CancelClicked.InvokeAsync();
    }

    private async Task Save()
    {
        await SaveClicked.InvokeAsync();
    }
}
