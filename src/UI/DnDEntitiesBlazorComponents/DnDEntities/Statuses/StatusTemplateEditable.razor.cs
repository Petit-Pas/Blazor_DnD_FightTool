using DnDFightTool.Domain.DnDEntities.Statuses;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Statuses;

public partial class StatusTemplateEditable
{
    [Parameter, EditorRequired]
    public StatusTemplate? StatusTemplate { get; set; }
}
