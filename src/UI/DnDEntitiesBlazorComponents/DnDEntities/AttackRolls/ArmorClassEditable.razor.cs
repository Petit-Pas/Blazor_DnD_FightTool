using DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.AttackRolls;

public partial class ArmorClassEditable : ComponentBase
{
    [Parameter, EditorRequired]
    public ArmorClass? ArmorClass { get; set; } = default;

    // UI Methods
    private string ShieldInputVisibility => ArmorClass!.HasShieldEquipped ? "" : "hidden";
}