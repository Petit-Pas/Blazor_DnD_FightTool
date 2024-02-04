using DnDFightTool.Domain.DnDEntities.AbilityScores;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace DnDEntitiesBlazorComponents.DnDEntities.AbilityScores;

public partial class AbilityScoresEditableList : ComponentBase
{
    [Parameter]
    public AbilityScoresCollection? Abilities { get; set; }

    [Parameter] public BorderRadius? BorderRadius { get; set; } = new (1, "em");
}