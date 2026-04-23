using CharacterSheetBlazorComponents.Damage.Components;
using CharacterSheetBlazorComponents.MartialAttacks.Components;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CharacterSheetBlazorComponents.MartialAttacks.Pages;

public partial class AttackEditorPage
{
    [Inject]
    public required IAttackEditContext AttackEditContext { get; set; }

    private MartialAttackTemplate? _attackTemplate { get; set; }
    
    private int _tabActiveIndex;
    private MartialAttackTemplateMainInfoEditorComponent? _mainInfoComponent;
    private DamageRollTemplateCollectionEditorComponent? _damageComponent;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _attackTemplate = AttackEditContext.Attack;
    }

    private async Task OnPreviewInteraction(TabInteractionEventArgs arg)
    {
        switch (_tabActiveIndex)
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
