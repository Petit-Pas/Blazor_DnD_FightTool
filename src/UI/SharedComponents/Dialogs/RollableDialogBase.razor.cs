using Microsoft.AspNetCore.Components;
using SharedComponents.Dices;

namespace SharedComponents.Dialogs;

public partial class RollableDialogBase : IDisposable
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool CanValidate { get; set; } = true;

    [Parameter]
    public EventCallback OnRollClick { get; set; }

    [Parameter]
    public EventCallback OnValidateClick { get; set; }

    /// <summary>
    ///     Injected notifier. Drives Roll button state and triggers <c>NotifyRollAsync</c> on click.
    /// </summary>
    [Inject]
    public IDiceRollNotifier? DiceRollNotifier { get; set; }

    /// <summary>
    ///     Effective enabled state of the Roll button.
    ///     When <see cref="OnRollClick"/> has a delegate the dialog manages rolling itself and the button is always enabled.
    ///     Otherwise the injected <see cref="IDiceRollNotifier"/> drives the state.
    /// </summary>
    internal bool IsRollEnabled => OnRollClick.HasDelegate || (DiceRollNotifier?.CanRoll ?? true);

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (DiceRollNotifier is not null)
        {
            DiceRollNotifier.StateChanged += OnNotifierStateChanged;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (DiceRollNotifier is not null)
        {
            DiceRollNotifier.StateChanged -= OnNotifierStateChanged;
        }
    }

    private void OnNotifierStateChanged() => InvokeAsync(StateHasChanged);

    protected async Task RollAsync()
    {
        if (OnRollClick.HasDelegate)
        {
            await OnRollClick.InvokeAsync();
            return;
        }

        if (DiceRollNotifier is not null)
        {
            await DiceRollNotifier.NotifyRollAsync();
        }
    }

    protected async Task ValidateAsync()
    {
        if (OnValidateClick.HasDelegate)
        {
            await OnValidateClick.InvokeAsync();
        }
    }
}
