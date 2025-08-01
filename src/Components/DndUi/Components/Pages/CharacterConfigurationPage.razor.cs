using DnDEntitiesBlazorComponents.DnDEntities.AbilityScores;
using DnDEntitiesBlazorComponents.DnDEntities.Characters.Components;
using DnDFightTool.Domain.DnDEntities.Characters;
using MudBlazor;

namespace DndUi.Components.Pages;

public partial class CharacterConfigurationPage
{
    private int _activeIndex;
    private Character _character = new(true);

    private CharacterMainInfoEditorComponent? _mainInfoComponent;
    private AbilityScoresEditorComponent? _abilityScoreComponent;

    private async Task OnPreviewInteraction(TabInteractionEventArgs arg)
    {
        switch (_activeIndex)
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
}
