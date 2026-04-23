using DnDFightTool.UI.CharacterSheetBlazorComponents.Dices.DiceRolls.Components;
using DnDFightTool.Domain.CharacterSheet.Damage;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.Damage.Components;

public partial class DamageRollTemplateEditorComponent
{
    [Parameter]
    public DamageRollTemplate? Template { get; set; }

    [Parameter]
    public EventCallback OnDeleted { get; set; }

    private DiceRollTemplateEditorComponent? _damageField;

    public bool Validate()
    {
        if (_damageField is null)
        {
            return true;
        }
        return _damageField.Validate();
    }

    private async Task Delete()
    {
        if (OnDeleted.HasDelegate)
        {
            await OnDeleted.InvokeAsync();
        }
    }
}
