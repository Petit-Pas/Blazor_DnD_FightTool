using DnDFightTool.Infrastructure.AspNetCoreExtensions.Navigations;
using DnDFightTool.UI.CharacterSheetBlazorComponents.AbilityScores;
using DnDFightTool.UI.CharacterSheetBlazorComponents.Characters.Components;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Infrastructure.Mapping;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.Characters.Pages;

public partial class CharacterEditorPage
{
    [Inject]
    public required ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }
    
    [Inject]
    public required IStateFullNavigation Navigation { get; set; }

    [Inject]
    private IGlobalEditContext GlobalEditContext { get; set; } = default!;

    private ICharacter? _character { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _character = GlobalEditContext.Character;
    }

    private int _previousTabIndex;

    private CharacterMainInfoEditorComponent? _mainInfoComponent;
    private AbilityScoresEditorComponent? _abilityScoreComponent;

    private async Task OnPreviewInteraction(TabInteractionEventArgs arg)
    {
        switch (_previousTabIndex)
        {
            case 0:
                ArgumentNullException.ThrowIfNull(_mainInfoComponent, nameof(_mainInfoComponent));
                if (!await _mainInfoComponent.ValidateAsync())
                {
                    arg.Cancel = true;
                }
                break;
            case 1:
                ArgumentNullException.ThrowIfNull(_abilityScoreComponent, nameof(_abilityScoreComponent));
                if (!await _abilityScoreComponent.ValidateAsync())
                {
                    arg.Cancel = true;
                }
                break;
            default:
                break;
        }

        if (!arg.Cancel)
        {
            _previousTabIndex = arg.PanelIndex;
        }
    }

    private async Task<bool> AreAllValid()
    {
        if ((await Task
            .WhenAll(
                _mainInfoComponent?.ValidateAsync() ?? Task.FromResult(true), 
                _abilityScoreComponent?.ValidateAsync() ?? Task.FromResult(true))
            ).Any(valid => !valid))
        {
            return false;
        }
        return true;
    }

    private async Task Save()
    {
        if (await AreAllValid())
        {
            GlobalEditContext.SaveEditedCharacter();
        }
    }

    private void Cancel()
    {
        GlobalEditContext.CancelCharacterEdittion();
    }
}
