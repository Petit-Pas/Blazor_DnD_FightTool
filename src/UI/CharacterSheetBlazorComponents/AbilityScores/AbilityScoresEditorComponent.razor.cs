using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Characters.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using DnDFightTool.UI.SharedComponents;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.AbilityScores;

public partial class AbilityScoresEditorComponent : StylableComponentBase
{
    [Inject]
    private CharacterValidator _characterValidator { get; set; } = null!;

    [Parameter, EditorRequired]
    public ICharacter? Character { get; set; }

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

    private MudForm? _mainForm;

    public async Task<bool> ValidateAsync()
    {
        if (_mainForm is null)
        {
            return false;
        }
        await _mainForm.ValidateAsync();
        return _mainForm.IsValid;
    }
}
