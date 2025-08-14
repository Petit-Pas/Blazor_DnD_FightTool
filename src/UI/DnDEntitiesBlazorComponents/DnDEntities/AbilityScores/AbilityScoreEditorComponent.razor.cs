using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.AbilityScores.Validation;
using DnDFightTool.Domain.DnDEntities.Characters;
using Microsoft.AspNetCore.Components;
using SharedComponents;

namespace DnDEntitiesBlazorComponents.DnDEntities.AbilityScores;

public partial class AbilityScoreEditorComponent
{
    [Inject]
#pragma warning disable 8618
    public AbilityScoreValidator AbilityScoreValidator { private get; set; }
#pragma warning restore 8618

    [Parameter, EditorRequired]
    public Character? Character { get; set; }

    [Parameter, EditorRequired]
    public AbilityScore? AbilityScore { get; set; }

    [Parameter]
    public EventCallback OnChanged { get; set; }

    // Call this when a value changes
    private async Task NotifyChangedAsync()
    {
        if (OnChanged.HasDelegate)
        {
            await OnChanged.InvokeAsync();
        }
    }
}
