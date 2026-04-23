using DnDFightTool.UI.CharacterSheetBlazorComponents.Dices.DiceRolls.Components;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.MartialAttacks.Components;

public partial class MartialAttackTemplateMainInfoEditorComponent
{
    [Inject]
    private MartialAttackTemplateValidator _attackTemplateValidator { get; set; } = null!;

    private MudForm? _form;

    [Parameter]
    public MartialAttackTemplate? AttackTemplate { get; set; }

    private DiceRollModifiersTemplateEditorComponent? _diceThrowEditor;

    public async Task<bool> ValidateAsync()
    {
        if (_form is not null && (_diceThrowEditor?.Validate() ?? true))
        {
            await _form.Validate();
            return _form.IsValid;
        }
        return false;
    }
}
