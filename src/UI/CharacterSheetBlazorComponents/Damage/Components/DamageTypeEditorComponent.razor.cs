using DnDFightTool.Domain.CharacterSheet.Damage;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.CharacterSheetBlazorComponents.Damage.Components;

public partial class DamageTypeEditorComponent
{
    [Parameter, EditorRequired]
    public required DamageTypeEnum DamageType { get; set; }

    [Parameter]
    public EventCallback<DamageTypeEnum> DamageTypeChanged { get; set; }

    protected async Task OnDamageTypeChanged(DamageTypeEnum value)
    {
        DamageType = value;
        if (DamageTypeChanged.HasDelegate)
        {
            await DamageTypeChanged.InvokeAsync(value);
        }
    }
}
