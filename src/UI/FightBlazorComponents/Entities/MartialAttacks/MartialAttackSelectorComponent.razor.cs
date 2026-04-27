using DnDFightTool.Business.DnDActions.MartialAttackActions.ExecuteMartialAttack;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using UndoableMediator.Mediators;

namespace DnDFightTool.UI.FightBlazorComponents.Entities.MartialAttacks;

public partial class MartialAttackSelectorComponent
{
    [Inject]
    public required IUndoableMediator Mediator { get; set; }

    [CascadingParameter(Name = "SelectedFighter")]
    private FightingCharacter? SelectedFighter { get; set; }

    private FightingCharacter? Character { get; set; }

    private MartialAttackTemplate? SelectedAttack { get; set; }

    protected override void OnParametersSet()
    {
        if (Character?.Id != SelectedFighter?.Id)
        {
            Character = SelectedFighter;
            SelectedAttack = null;
        }
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

    private async Task AttackAsync()
    {
        if (Character is null || SelectedAttack is null)
        {
            return;
        }

        await InvokeAsync(() => Mediator.SendAsync(new ExecuteMartialAttackCommand(Character.Id, SelectedAttack.Id)));
    }
}
