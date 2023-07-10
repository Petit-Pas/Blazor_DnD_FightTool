using DnDEntities.Characters;
using DnDEntities.Dices.DiceThrows;
using Microsoft.AspNetCore.Components;

namespace DnDEntitiesBlazorComponents.DnDEntities.Dices.DiceThrows.Components;

public partial class HitRollResultEditable
{
    [Parameter]
    public HitRollResult HitRollResult { get; set; }

    [Parameter]
    public Character Caster { get; set; }
}
