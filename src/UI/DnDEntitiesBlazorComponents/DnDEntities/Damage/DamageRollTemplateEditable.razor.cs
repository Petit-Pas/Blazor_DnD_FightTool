using DnDFightTool.Domain.DnDEntities.Damage;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Damage;

public partial class DamageRollTemplateEditable
{
    [Parameter, EditorRequired]
    public DamageRollTemplate? DamageRollTemplate { get; set; }
}
