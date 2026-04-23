using DnDFightTool.Domain.CharacterSheet.Damage;
using Microsoft.AspNetCore.Components;

namespace DnDFightTool.UI.FightBlazorComponents.Entities.Damage;

/// <summary>
///     Renders a damage type icon with a tooltip showing the damage type name.
/// </summary>
public partial class DamageTypeIconComponent
{
    [Parameter]
    public DamageTypeEnum DamageType { get; set; }
}
