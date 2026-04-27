using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Events.AppliedStatusUpdated;
using Microsoft.AspNetCore.Components;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components.Web;
using DnDFightTool.UI.CharacterSheetBlazorComponents;
using DnDFightTool.Infrastructure.Mapping;
using MudBlazor;
using DnDFightTool.Domain.Fight.DomainExtensions.HitPoint;

namespace DnDFightTool.UI.FightBlazorComponents.Entities.FightingCharacters.Components;

public partial class FightingCharacterTile : ComponentBase, IDisposable
{
    [Inject]
    public required IFightContext FightContext { get; set; }

    [Inject]
    public required IAppliedStatusRepository AppliedStatusRepository { get; set; }

    [Inject]
    public required IGlobalEditContext GlobalEditContext { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Parameter]
    public required FightingCharacter Fighter { get; set; }

    [Parameter]
    public EventCallback<FightingCharacter> OnSelected { get; set; }

    [CascadingParameter(Name = "SelectedFighter")]
    private FightingCharacter? SelectedFighter { get; set; }

    private bool _isSelected = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        FightContext.OnFighterUpdated += OnFighterUpdated;
        AppliedStatusRepository.AppliedStatusUpdated += OnAppliedStatusUpdated;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FightContext.OnFighterUpdated -= OnFighterUpdated;
        AppliedStatusRepository.AppliedStatusUpdated -= OnAppliedStatusUpdated;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        _isSelected = SelectedFighter?.Id == Fighter?.Id;
    }

    private async void OnFighterUpdated(object? sender, Guid fighterId)
    {
        if (fighterId == Fighter.Id)
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private async void OnAppliedStatusUpdated(object sender, AppliedStatusUpdatedEventArgs e)
    {
        if (e.AffectedCharacterId == Fighter.Id)
        {
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task CardClicked(MouseEventArgs _)
    {
        await OnSelected.InvokeAsync(Fighter);
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