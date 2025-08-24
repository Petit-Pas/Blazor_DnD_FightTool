using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.DamageAffinities;
using Microsoft.AspNetCore.Components;
using SharedComponents;
using SharedComponents.Icons;

namespace DnDEntitiesBlazorComponents.DnDEntities.DamageAffinities.Components;

public partial class ResistancesEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public Character? Character { get; set; } = default!;
}
