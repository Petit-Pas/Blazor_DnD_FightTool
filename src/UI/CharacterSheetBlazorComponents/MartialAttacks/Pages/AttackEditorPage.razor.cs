using DnDFightTool.UI.CharacterSheetBlazorComponents.Damage.Components;
using DnDFightTool.UI.CharacterSheetBlazorComponents.MartialAttacks.Components;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.MartialAttacks.Pages;

public partial class AttackEditorPage
{
    [Inject]
    public required IAttackEditContext AttackEditContext { get; set; }

    private MartialAttackTemplate? _attackTemplate { get; set; }
    
    private int _previousTabIndex;
    private MartialAttackTemplateMainInfoEditorComponent? _mainInfoComponent;
    private DamageRollTemplateCollectionEditorComponent? _damageComponent;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _attackTemplate = AttackEditContext.Attack;
    }

    private async Task OnPreviewInteraction(TabInteractionEventArgs arg)
    {
        switch (_previousTabIndex)
        {
            case 0:
                ArgumentNullException.ThrowIfNull(_mainInfoComponent, nameof(_mainInfoComponent));
                ArgumentNullException.ThrowIfNull(_damageComponent, nameof(_damageComponent));
                if (!await _mainInfoComponent.ValidateAsync() || _damageComponent.Validate())
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
                Task.FromResult(_damageComponent?.Validate() ?? true))
            ).Any(valid => !valid))
        {
            return false;
        }
        return true;
    }

    protected async Task Save()
    {
        if (await AreAllValid())
        {
            AttackEditContext.SaveEditedAttack();
        }
    }

    protected void Cancel()
    {
        AttackEditContext.CancelAttackEdition();
    }
}
