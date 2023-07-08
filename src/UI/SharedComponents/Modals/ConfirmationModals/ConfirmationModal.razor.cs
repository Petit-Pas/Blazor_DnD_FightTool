using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace SharedComponents.Modals.ConfirmationModals;

public partial class ConfirmationModal
{
    [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter]
    public string Message { get; set; }

    [Parameter]
    public string Title { get; set; }

    public async Task Close()
    {
        await BlazoredModal.CloseAsync(ModalResult.Ok());
    }

    private async Task Cancel()
    {
        await BlazoredModal.CancelAsync(ModalResult.Cancel());
    }
}