﻿using DnDFightTool.Domain.DnDEntities.Damage;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.Components.NeoPopover;

namespace DnDEntitiesBlazorComponents.DnDEntities.Damage;

public partial class DamageTypePicker
{
    /// <summary>
    ///     Property to bind to for the damage type
    /// </summary>
    [Parameter, EditorRequired]
    public DamageTypeEnum DamageType { get; set; }

    /// <summary>
    ///     Event used for the DamageType propertyBinding
    /// </summary>
    [Parameter]
    public virtual EventCallback<DamageTypeEnum> DamageTypeChanged { get; set; }

    private NeoPopover _popOver { get; set; }

    private async Task OnValueChanged(DamageTypeEnum damageType)
    {
        if (damageType != DamageType)
        {
            await DamageTypeChanged.InvokeAsync(damageType);
        }
    }

    private async Task UpdateSelectedElement(DamageTypeEnum selected)
    {
        await OnValueChanged(selected);
        await _popOver.TogglePopoverAsync();
    }
}
