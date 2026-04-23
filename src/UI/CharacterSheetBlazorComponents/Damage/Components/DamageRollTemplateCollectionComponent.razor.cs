using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Damage;
using Microsoft.AspNetCore.Components;

namespace CharacterSheetBlazorComponents.Damage.Components;

public partial class DamageRollTemplateCollectionComponent
{
    [Parameter]
    public ICharacter? Character { get; set; }

    [Parameter]
    public DamageRollTemplateCollection? DamageRolls { get; set; }
}
