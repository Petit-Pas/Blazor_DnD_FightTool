using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight;
using Microsoft.AspNetCore.Components;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components.Web;
using NeoBlazorphic.StyleParameters;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;

namespace FightBlazorComponents.Entities.FightingCharacters.Components;

public partial class FightingCharacterTile : ComponentBase, IDisposable
{
    [Inject]
    public required ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public required IFightContext FightContext { get; set; }

    [Inject]
    public required IAppliedStatusRepository AppliedStatusCollection { get; set; }

    [Parameter]
    public required FightingCharacter Fighter { get; set; }

    private FightingCharacter? _character = null;

    private readonly static BorderRadius _borderRadius = new(2, "em");

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitCharacter();

        FightContext.ActiveFighterChanged += OnMovingCharacterChanged;
        AppliedStatusCollection.AppliedStatusUpdated += AppliedStatusCollection_AppliedStatusUpdated;
    }

    private void AppliedStatusCollection_AppliedStatusUpdated(object _, AppliedStatusUpdatedEventArgs e)
    {
        if (e.AffectedCharacterId == _character?.Id)
        {
            // TODO the refresh works without that, but I think its because the whole state is recomputed when the HPs change, to try with an attack that has no damage 
            StateHasChanged();
        }
    }

    private void OnMovingCharacterChanged(object? sender, FightingCharacter? fightingCharacter)
    {
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        InitCharacter();
    }

    private void InitCharacter()
    {
        if (_character == null || _character.Id != Fighter.Id)
        {
            _character = FightContext[Fighter.Id];
        }
    }

    public void Dispose()
    {
        FightContext.ActiveFighterChanged -= OnMovingCharacterChanged;
        AppliedStatusCollection.AppliedStatusUpdated -= AppliedStatusCollection_AppliedStatusUpdated;
        GC.SuppressFinalize(this);
    }

    private void TileClicked(MouseEventArgs _)
    {
        FightContext.SetActiveFighter(Fighter);
    }

    // UI Methods
    private ThemeColor CardTheme => FightContext.ActiveFighter == Fighter
        ? ThemeColor.Primary 
        : ThemeColor.Base;
}