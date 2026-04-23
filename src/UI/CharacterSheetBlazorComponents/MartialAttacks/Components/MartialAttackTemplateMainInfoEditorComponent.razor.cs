using CharacterSheetBlazorComponents.Dices.DiceRolls.Components;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CharacterSheetBlazorComponents.MartialAttacks.Components;

public partial class MartialAttackTemplateMainInfoEditorComponent
{
    [Inject]
#pragma warning disable 8618
    public MartialAttackTemplateValidator AttackTemplateValidator { private get; set; }
#pragma warning restore 8618

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
