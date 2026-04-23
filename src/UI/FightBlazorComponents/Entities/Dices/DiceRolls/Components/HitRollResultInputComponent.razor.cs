using DnDFightTool.Domain.Rolls;
using Microsoft.AspNetCore.Components;
using SharedComponents;

namespace FightBlazorComponents.Entities.Dices.DiceRolls.Components;

/// <summary>
///     Input component for a roll result.
///     Shows [d20 input with Min/Max bounds] + modifier expression.
/// </summary>
public partial class HitRollResultInputComponent : StylableComponentBase
{
    /// <summary>
    ///     The roll result to edit.
    /// </summary>
    [Parameter]
    public HitRollResult? RollResult { get; set; }
}
