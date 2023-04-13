﻿using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace SharedComponents.Modals.Common;

public partial class ModalCloseButton
{
    [Parameter]
    public virtual EventCallback<MouseEventArgs> OnMouseClickCallBack { get; set; }

    protected virtual async Task OnMouseClick(MouseEventArgs args)
    {
        await OnMouseClickCallBack.InvokeAsync(args);
    }

}