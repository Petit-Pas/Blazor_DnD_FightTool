using DnDFightTool.Domain.DnDEntities.Saves;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Saves;

public partial class SaveRollTemplateEditable
{
    [Parameter, EditorRequired]
    public SaveRollTemplate? SaveRollTemplate { get; set; }
}
