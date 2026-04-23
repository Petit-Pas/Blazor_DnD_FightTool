using DnDFightTool.Domain.Fight;
using Microsoft.AspNetCore.Components;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components.Web;
using DnDEntitiesBlazorComponents;
using Mapping;
using MudBlazor;
using DnDFightTool.Domain.Fight.DomainExtensions.HitPoint;

namespace FightBlazorComponents.Entities.FightingCharacters.Components;

public partial class FightingCharacterTile : ComponentBase, IDisposable
{
    [Inject]
    public required IFightContext FightContext { get; set; }

    [Inject]
    public required IGlobalEditContext GlobalEditContext { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Parameter]
    public required FightingCharacter Fighter { get; set; }

    private bool _isSelected = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        FightContext.OnActiveFighterChanged += OnActiveFighterChanged;
        FightContext.OnFighterUpdated += OnFighterUpdated;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FightContext.OnActiveFighterChanged -= OnActiveFighterChanged;
        FightContext.OnFighterUpdated -= OnFighterUpdated;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Fighter == FightContext.ActiveFighter)
        {
            _isSelected = true;
        }
    }

    private void OnActiveFighterChanged(object? sender, FightingCharacter? e)
    {
        _isSelected = e == Fighter;
        StateHasChanged();
    }

    private async void OnFighterUpdated(object? sender, Guid fighterId)
    {
        if (fighterId == Fighter.Id)
            await InvokeAsync(StateHasChanged);
    }

    private void CardClicked(MouseEventArgs _)
    {
        FightContext.SetActiveFighter(Fighter);
    }

    private void Edit()
    {
        GlobalEditContext.EditCharacter(Fighter.Copy(Mapper));
    }

    private void Delete()
    {
        FightContext.Remove(Fighter);
    }

    private Color GetHealthBarColor()
    {
        return Fighter.HitPoints.GetHealthRatio() switch
        {
            >= 50 => Color.Success,
            >= 25 => Color.Warning,
            _ => Color.Error
        };
    }

}