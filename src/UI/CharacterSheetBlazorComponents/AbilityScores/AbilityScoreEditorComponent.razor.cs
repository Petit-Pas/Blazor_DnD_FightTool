using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.AbilityScores.Validation;
using DnDFightTool.Domain.CharacterSheet.Characters;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.AbilityScores;

public partial class AbilityScoreEditorComponent
{
    [Inject]
    private AbilityScoreValidator _abilityScoreValidator { get; set; } = null!;

    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; }

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
