using Characters.AbilityScores;
using Microsoft.AspNetCore.Components;

namespace DnDBlazorComponents.Characters.AbilityScores;

public partial class AbilityScoresEditableList : ComponentBase
{
    [Parameter]
    public AbilityScoresCollection? Abilities { get; set; }
}