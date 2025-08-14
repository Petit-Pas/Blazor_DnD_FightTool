using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Characters.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedComponents;

namespace DnDEntitiesBlazorComponents.DnDEntities.AbilityScores;

public partial class AbilityScoresEditorComponent : StylableComponentBase
{
    [Inject]
#pragma warning disable 8618
    public CharacterValidator CharacterValidator { private get; set; }
#pragma warning restore 8618

    [Parameter, EditorRequired]
    public Character? Character { get; set; }

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
        await _mainForm.Validate();
        return _mainForm.IsValid;
    }
}
