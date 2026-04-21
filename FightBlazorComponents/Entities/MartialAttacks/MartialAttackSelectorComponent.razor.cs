using DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.Fight;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UndoableMediator.Mediators;

namespace FightBlazorComponents.Entities.MartialAttacks;

public partial class MartialAttackSelectorComponent : IDisposable
{
    [Inject]
    public required IFightContext FightContext { get; set; }

    [Inject]
    public required IUndoableMediator Mediator { get; set; }

    private FightingCharacter? Character { get; set; }

    private MartialAttackTemplate? SelectedAttack { get; set; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        FightContext.OnActiveFighterChanged -= FightContext_OnActiveFighterChanged;
    }

    protected override void OnInitialized()
    {
        Character = FightContext.ActiveFighter;

        FightContext.OnActiveFighterChanged += FightContext_OnActiveFighterChanged;
    }

    private async Task OnAttackClicked(TableRowClickEventArgs<MartialAttackTemplate> tableRowClickEventArgs)
    {
        SelectedAttack = tableRowClickEventArgs.Item;
        if (tableRowClickEventArgs.MouseEventArgs.Detail > 1)
        {
            await InvokeAsync(AttackAsync);
        }
    }

    private string SelectedRowClassFunc(MartialAttackTemplate attackTemplate, int _)
    {
        return attackTemplate == SelectedAttack ? "selected-attack" : ""; 
    }

    private void FightContext_OnActiveFighterChanged(object? _, FightingCharacter? e)
    {
        SelectedAttack = null;
        Character = e;
        StateHasChanged();
    }

    private async Task AttackAsync()
    {
        if (Character is null || SelectedAttack is null)
        {
            return;
        }

        await InvokeAsync(() => Mediator.Execute(new ExecuteMartialAttackCommand(Character.Id, SelectedAttack.Id)));
    }
}
