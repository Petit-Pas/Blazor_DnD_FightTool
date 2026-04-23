using DnDFightTool.Domain.CharacterSheet.Characters;
using Microsoft.AspNetCore.Components;
using SharedComponents;

namespace CharacterSheetBlazorComponents.Skills.Components;

public partial class SkillsEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; }
}
