using DnDEntities.Characters;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.MartialAttacks;

public partial class MartialAttackTemplatesEditable
{
    [Parameter]
    public Character Character { get; set; }

}
