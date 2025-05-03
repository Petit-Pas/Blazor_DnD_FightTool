using System.Linq.Expressions;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;

namespace FightBlazorComponents.Entities.FightingCharacters.Components;

public partial class FightingCharacterPicker
{
    [Inject]
    public IFightContext? FightContext { get; set; }

    private Fighter[]? _fightingCharacters = null;

    [Parameter]
    public Guid SelectedCharacterId { get; set; } = Guid.Empty;

    [Parameter]
    public EventCallback<Guid> SelectedCharacterIdChanged { get; set; }
    
    [Parameter, EditorRequired]
    public virtual Expression<Func<Guid>> ValidationFor { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_fightingCharacters == null && FightContext != null)
        {
            _fightingCharacters = [.. FightContext.GetFighters()];
            StateHasChanged();
        }
    }

    private async Task OnTargetChanged(ChangeEventArgs eventArgs)
    {
        if (Guid.TryParse(eventArgs.Value as string, out var selectedGuid))
        {
            await SelectedCharacterIdChanged.InvokeAsync(selectedGuid);
        }
    }
}
