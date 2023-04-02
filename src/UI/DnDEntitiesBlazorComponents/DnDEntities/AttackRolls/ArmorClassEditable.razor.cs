using DnDEntities.AttackRolls.ArmorClasses;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.AttackRolls;

public partial class ArmorClassEditable : ComponentBase
{
    [Parameter, EditorRequired]
    public ArmorClass ArmorClass { get; set; } = ArmorClass.Default;

    // UI Methods
    private string ShieldInputVisibility => ArmorClass.HasShieldEquipped ? "" : "hidden";
}