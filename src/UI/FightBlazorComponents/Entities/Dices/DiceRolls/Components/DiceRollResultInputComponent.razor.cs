using DnDFightTool.Domain.DnDEntities.Dices;
using DnDFightTool.Domain.DnDEntities.Dices.Validation;
using Microsoft.AspNetCore.Components;
using SharedComponents;

namespace FightBlazorComponents.Entities.Dices.DiceRolls.Components;

/// <summary>
///     Generic input component for any <see cref="IDiceRollResult"/>.
///     Renders a numeric field validated through a <see cref="DiceRollResultValidator"/>.
/// </summary>
public partial class DiceRollResultInputComponent : StylableComponentBase
{
    [Inject]
    public DiceRollResultValidator Validator { get; set; } = null!;

    /// <summary>
    ///     The dice roll result to edit.
    /// </summary>
    [Parameter]
    public IDiceRollResult? RollResult { get; set; }

    /// <summary>
    ///     Callback invoked when the result value changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    /// <summary>
    ///     Label for the numeric field (e.g. "d20", "Damage").
    /// </summary>
    [Parameter]
    public string Label { get; set; } = "d20";

    private async Task OnValueChanged(int value)
    {
        if (RollResult is not null)
        {
            RollResult.Result = value;
        }

        await ValueChanged.InvokeAsync(value);
    }
}
