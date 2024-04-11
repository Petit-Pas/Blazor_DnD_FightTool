using DnDFightTool.Domain.DnDEntities.Statuses;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Statuses;

public partial class OnHitStatusTemplateCollectionEditable : ComponentBase
{
    [Parameter]
    public StatusTemplateCollection? OnHitStatusTemplateCollection { get; set; }
}
