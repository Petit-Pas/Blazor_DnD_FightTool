using DnDFightTool.Domain.CharacterSheet.Characters;
using Microsoft.AspNetCore.Components;
using DnDFightTool.UI.SharedComponents;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.DamageAffinities.Components;

public partial class ResistancesEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; } = default!;
}
