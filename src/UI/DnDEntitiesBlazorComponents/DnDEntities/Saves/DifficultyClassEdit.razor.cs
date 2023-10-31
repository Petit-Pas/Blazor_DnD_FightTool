using DnDFightTool.Domain.DnDEntities.Saves;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Saves;

public partial class DifficultyClassEdit
{
    [Parameter, EditorRequired]
    public DifficultyClass? DifficultyClass { get; set; }
}
