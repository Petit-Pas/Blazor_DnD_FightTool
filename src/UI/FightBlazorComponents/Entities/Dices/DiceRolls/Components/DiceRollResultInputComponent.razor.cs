using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.Rolls.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedComponents;
using SharedComponents.Dices;

namespace FightBlazorComponents.Entities.Dices.DiceRolls.Components;

/// <summary>
///     Generic input component for any <see cref="IDiceRollResult"/>.
///     Renders a numeric field validated through a <see cref="DiceRollResultValidator"/>.
///     Self-registers with an <see cref="IDiceRollNotifier"/>
///     allowing a parent dialog to trigger auto-rolling via the Roll button.
/// </summary>
public partial class DiceRollResultInputComponent : StylableComponentBase, IDiceRollSubscriber, IDisposable
{
    [Inject]
    public DiceRollResultValidator Validator { get; set; } = null!;

    /// <summary>
    ///     Optional notifier injected from DI.
    ///     When present, this component self-registers so it can be asked to roll automatically.
    /// </summary>
    [Inject]
    public IDiceRollNotifier? DiceRollNotifier { get; set; }

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

    private MudForm? _form;

    /// <summary>
    ///     <c>true</c> when the roll result has actual dice to roll (Min > 0 or Max > 0).
    ///     When <c>false</c>, the input is hidden and the component is considered always rolled.
    /// </summary>
    private bool HasDice => RollResult is not null && (RollResult.Min > 0 || RollResult.Max > 0);

    /// <inheritdoc />
    public bool IsRolled => !HasDice || (RollResult?.IsRolled ?? true);

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        DiceRollNotifier?.Subscribe(this);

        if (RollResult is IRollRangeChangedNotifier rangeNotifier)
        {
            rangeNotifier.RangeChanged += OnRangeChanged;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DiceRollNotifier?.Unsubscribe(this);

        if (RollResult is IRollRangeChangedNotifier rangeNotifier)
        {
            rangeNotifier.RangeChanged -= OnRangeChanged;
        }
    }

    private void OnRangeChanged() => InvokeAsync(HandleRangeChangedAsync);

    private async Task HandleRangeChangedAsync()
    {
        // Only re-validate when the field has already been touched (Result != 0).
        // Untouched fields just need a silent notifier refresh so CanValidate updates.
        if (RollResult is not null && RollResult.Result != 0 && _form is not null)
        {
            await _form.Validate();
        }

        DiceRollNotifier?.RaiseStateChanged();
    }

    /// <inheritdoc />
    public async Task RollAsync()
    {
        if (RollResult is null)
        {
            return;
        }

        RollResult.Roll();

        if (_form is not null)
        {
            await _form.Validate();
        }

        await ValueChanged.InvokeAsync(RollResult.Result);
    }

    private async Task OnValueChanged(int value)
    {
        if (RollResult is not null)
        {
            RollResult.Result = value;
        }

        await ValueChanged.InvokeAsync(value);
        DiceRollNotifier?.RaiseStateChanged();
    }
}

