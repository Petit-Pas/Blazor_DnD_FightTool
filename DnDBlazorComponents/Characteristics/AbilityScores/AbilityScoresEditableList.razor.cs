using Characters.AbilityScores;
using Microsoft.AspNetCore.Components;

namespace DnDBlazorComponents.Characteristics.AbilityScores
{
    public partial class AbilityScoresEditableList : ComponentBase
    {
        [Parameter]
        public AbilityScoresCollection Model { get; set; }
    }
}
