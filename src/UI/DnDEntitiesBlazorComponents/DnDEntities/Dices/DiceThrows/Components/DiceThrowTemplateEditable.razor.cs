using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Dices.DiceThrows.Components;

public partial class DiceThrowTemplateEditable
{
    [Parameter]
    public DiceThrowTemplate Template { get; set; }
}
