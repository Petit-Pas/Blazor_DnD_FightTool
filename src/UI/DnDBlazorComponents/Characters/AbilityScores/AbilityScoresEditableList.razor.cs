using Characters.AbilityScores;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace DnDBlazorComponents.Characters.AbilityScores;

public partial class AbilityScoresEditableList : ComponentBase
{
    [Parameter]
    public AbilityScoresCollection? Abilities { get; set; }

    [Parameter] public BorderRadius? BorderRadius { get; set; } = new (1, "em");
}