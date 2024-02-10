using DnDFightTool.Domain.DnDEntities.Damage;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Damage
{
    public partial class DamageRollTemplateCollectionEditable
    {
        [Parameter]
        public DamageRollTemplateCollection? DamageRollTemplateCollection { get; set; }
    }
}
