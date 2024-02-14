using DnDFightTool.Domain.DnDEntities.Damage;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.Components.NeoPopover;

namespace DnDEntitiesBlazorComponents.DnDEntities.Damage;

public partial class DamageRollTemplateEditable
{
    [Parameter]
    public virtual DamageRollTemplate? Template { get; set; }

    [Parameter]
    public virtual EventCallback Deleted { get; set; }

    private NeoPopover _popOver { get; set; }

    private async Task NotifyDeleted()
    {
        await Deleted.InvokeAsync();
    }

    private async Task UpdateType(DamageTypeEnum newType)
    {
        if (Template is not null)
        {
            Template.Type = newType;
        }
        await _popOver.TogglePopover();
    }

}
