using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Damage;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Damage.Components;

public partial class DamageRollTemplateCollectionComponent
{
    [Parameter]
    public ICharacter? Character { get; set; }

    [Parameter]
    public DamageRollTemplateCollection? DamageRolls { get; set; }
}
