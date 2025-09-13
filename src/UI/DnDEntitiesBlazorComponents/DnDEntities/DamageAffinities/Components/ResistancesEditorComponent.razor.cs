using DnDFightTool.Domain.DnDEntities.Characters;
using Microsoft.AspNetCore.Components;
using SharedComponents;

namespace DnDEntitiesBlazorComponents.DnDEntities.DamageAffinities.Components;

public partial class ResistancesEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; } = default!;
}
