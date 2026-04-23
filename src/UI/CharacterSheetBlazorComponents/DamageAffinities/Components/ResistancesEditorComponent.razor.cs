using DnDFightTool.Domain.CharacterSheet.Characters;
using Microsoft.AspNetCore.Components;
using SharedComponents;

namespace CharacterSheetBlazorComponents.DamageAffinities.Components;

public partial class ResistancesEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; } = default!;
}
