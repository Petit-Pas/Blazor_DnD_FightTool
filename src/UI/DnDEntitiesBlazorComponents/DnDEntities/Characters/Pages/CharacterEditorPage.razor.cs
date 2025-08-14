using AspNetCoreExtensions.Navigations;
using DnDEntitiesBlazorComponents.DnDEntities.AbilityScores;
using DnDEntitiesBlazorComponents.DnDEntities.Characters.Components;
using DnDFightTool.Domain.DnDEntities.Characters;
using Mapping;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DnDEntitiesBlazorComponents.DnDEntities.Characters.Pages;

public partial class CharacterEditorPage
{
    [Inject]
    public required ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }
    
    [Inject]
    public required IStateFullNavigation Navigation { get; set; }

    [Inject]
    IGlobalEditContext GlobalEditContext { get; set; } = default!;

    private Character? _character { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _character = GlobalEditContext.Character;
    }

    private int _tabActiveIndex;

    private CharacterMainInfoEditorComponent? _mainInfoComponent;
    private AbilityScoresEditorComponent? _abilityScoreComponent;

    private async Task OnPreviewInteraction(TabInteractionEventArgs arg)
    {
        switch (_tabActiveIndex)
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
    }

    private void Save()
    {
        CharacterRepository.Save(_character!);
        Navigation.NavigateBack();
    }

    private void Cancel()
    {
        Navigation.NavigateBack();
    }
}
