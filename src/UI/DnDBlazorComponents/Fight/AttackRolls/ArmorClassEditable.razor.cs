using Fight.AttackRolls.ArmorClasses;
using Microsoft.AspNetCore.Components;

namespace DnDBlazorComponents.Fight.AttackRolls;

public partial class ArmorClassEditable : ComponentBase
{
    [Parameter, EditorRequired]
    public ArmorClass ArmorClass { get; set; }

    // UI Methods
    protected string ShieldInputVisibility => ArmorClass.HasShieldEquipped ? "" : "hidden";
}