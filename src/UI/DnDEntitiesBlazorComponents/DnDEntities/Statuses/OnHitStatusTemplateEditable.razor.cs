﻿using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Statuses;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.Components.NeoPopover;

namespace DnDEntitiesBlazorComponents.DnDEntities.Statuses;

public partial class OnHitStatusTemplateEditable
{
    [Parameter]
    public virtual StatusTemplate? Template { get; set; }

    [Parameter]
    public virtual EventCallback Deleted { get; set; }

    private void NotifyDeleted()
    {
        Deleted.InvokeAsync();
    }

    private NeoPopover _popOver { get; set; }

    private void OnPopoverOpenedChanged()
    {
        StateHasChanged();
    }

    private async Task UpdateAbility(AbilityEnum newAbility)
    {
        if (Template is not null)
        {
            Template.Save.TargetAbility = newAbility;
        }
        await _popOver.TogglePopoverAsync();
    }
}
