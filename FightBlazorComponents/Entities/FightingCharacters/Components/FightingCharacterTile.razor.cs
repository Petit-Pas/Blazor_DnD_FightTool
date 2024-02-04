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
    public ICharacterRepository CharacterRepository { get; set; }

    [Inject]
    public IFightContext FightContext { get; set; }

    [Parameter]
    public Fighter Fighter { get; set; }

    [Inject]
    public IAppliedStatusRepository AppliedStatusCollection { get; set; }

    private Character? Character = null;

    private BorderRadius BorderRadius = new (2, "em");

    protected override void OnInitialized()
    {
        base.OnInitialized();
        InitCharacter();

        FightContext.MovingFighterChanged += OnMovingCharacterChanged;
        AppliedStatusCollection.AppliedStatusUpdated += AppliedStatusCollection_AppliedStatusUpdated;
    }

    private void AppliedStatusCollection_AppliedStatusUpdated(object _, AppliedStatusUpdatedEventArgs e)
    {
        if (e.AffectedCharacterId == Character?.Id)
        {
            // TODO the refresh works without that, but I think its because the whole state is recomputed when the HPs change, to try with an attack that has no damage 
            StateHasChanged();
        }
    }

    private void OnMovingCharacterChanged(object? sender, Fighter? fightingCharacter)
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
        if (Character == null || Character.Id != Fighter.CharacterId)
        {
            Character = FightContext.GetCharacterById(Fighter.CharacterId);
        }
    }

    public void Dispose()
    {
        FightContext.MovingFighterChanged -= OnMovingCharacterChanged;
        AppliedStatusCollection.AppliedStatusUpdated -= AppliedStatusCollection_AppliedStatusUpdated;
    }

    private void TileClicked(MouseEventArgs _)
    {
        FightContext.SetMovingFighter(Fighter);
    }

    // UI Methods
    private ThemeColor CardTheme => FightContext.MovingFighter == Fighter
        ? ThemeColor.Primary 
        : ThemeColor.Base;
}