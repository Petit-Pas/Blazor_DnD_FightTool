using DnDFightTool.Domain.CharacterSheet.Characters;
using Microsoft.AspNetCore.Components;
using DnDFightTool.UI.SharedComponents;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.Skills.Components;

public partial class SkillsEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; }
}
