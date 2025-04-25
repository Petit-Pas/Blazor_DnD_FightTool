
using Blazored.Modal;
using Microsoft.AspNetCore.Components;

namespace SharedComponents.Modals.Templates;

public partial class ValidateCancelModal
{
    [CascadingParameter]
    public required BlazoredModalInstance BlazoredModal { get; set; }

    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    [Parameter]
    public required Func<bool> CanBeValidated { get; set; }

    private async Task Validate()
    {
        if (CanBeValidated())
        {
            await BlazoredModal.CloseAsync();
        }
    }

    private async Task Cancel()
    {
        await BlazoredModal.CancelAsync();
    }
}
