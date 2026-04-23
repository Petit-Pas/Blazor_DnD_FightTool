namespace SharedComponents.Dices;

/// <summary>
///     Coordinates roll orchestration between a parent dialog and its <see cref="IDiceRollSubscriber"/> children.
///     Provide a concrete implementation as a cascading value (typed as <see cref="IDiceRollNotifier"/>)
///     so both <c>RollableDialogBase</c> and every <c>DiceRollResultInputComponent</c> can participate.
/// </summary>
public interface IDiceRollNotifier
{
    /// <summary>
    ///     <c>true</c> while at least one subscriber has not yet been rolled.
    ///     Drives the enabled state of the Roll button in <c>RollableDialogBase</c>.
    /// </summary>
    bool CanRoll { get; }

    /// <summary>
    ///     Raised after any subscriber rolls or its roll state changes (e.g. manual edit).
    ///     Listeners should call <c>StateHasChanged()</c> on receipt.
    /// </summary>
    event Action? StateChanged;

    /// <summary>
    ///     Registers a subscriber. Called from <c>OnInitialized</c> of the subscriber component.
    /// </summary>
    void Subscribe(IDiceRollSubscriber subscriber);

    /// <summary>
    ///     Unregisters a subscriber. Called from <c>Dispose</c> of the subscriber component.
    /// </summary>
    void Unsubscribe(IDiceRollSubscriber subscriber);

    /// <summary>
    ///     Asks every registered subscriber to roll in sequence.
    ///     Called by <c>RollableDialogBase</c> when the Roll button is clicked.
    /// </summary>
    Task NotifyRollAsync();

    /// <summary>
    ///     Signals listeners that a subscriber's state has changed (e.g. value typed manually).
    ///     Called by subscriber components after each value change.
    /// </summary>
    void RaiseStateChanged();
}
