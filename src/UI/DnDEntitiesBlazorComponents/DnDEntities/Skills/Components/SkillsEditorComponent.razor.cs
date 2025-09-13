using DnDFightTool.Domain.DnDEntities.Characters;
using Microsoft.AspNetCore.Components;
using SharedComponents;

namespace DnDEntitiesBlazorComponents.DnDEntities.Skills.Components;

public partial class SkillsEditorComponent : StylableComponentBase
{
    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; }
}
