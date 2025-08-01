﻿using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace SharedComponents.Modals.Templates;

public partial class ConfirmationModal
{
    [CascadingParameter] private BlazoredModalInstance BlazoredModal { get; set; } = default!;

    [Parameter]
    public required string Message { get; set; }

    [Parameter]
    public required string Title { get; set; }

    public async Task Close()
    {
        await BlazoredModal.CloseAsync(ModalResult.Ok());
    }

    private async Task Cancel()
    {
        await BlazoredModal.CancelAsync(ModalResult.Cancel());
    }
}