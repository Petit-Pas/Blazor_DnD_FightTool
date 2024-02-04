
using System.ComponentModel.DataAnnotations;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;

namespace SharedComponents.Modals.Templates;

public partial class ValidateCancelModal
{
    [CascadingParameter]
    BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter, Required]
    public Func<bool> CanBeValidated { get; set; }

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
