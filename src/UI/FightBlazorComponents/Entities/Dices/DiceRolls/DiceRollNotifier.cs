using DnDFightTool.UI.SharedComponents.Dices;

namespace DnDFightTool.UI.FightBlazorComponents.Entities.Dices.DiceRolls;

/// <summary>
///     Default implementation of <see cref="IDiceRollNotifier"/>.
///     The parent dialog creates an instance and provides it as a cascading value
///     typed as <see cref="IDiceRollNotifier"/> so both <c>RollableDialogBase</c>
///     and every <c>DiceRollResultInputComponent</c> can participate without
///     depending on the concrete type.
/// </summary>
public class DiceRollNotifier : IDiceRollNotifier
{
    private readonly List<IDiceRollSubscriber> _subscribers = [];

    /// <inheritdoc />
    public event Action? StateChanged;

    /// <inheritdoc />
    public bool CanRoll => _subscribers.Count == 0 || !_subscribers.All(s => s.IsRolled);

    /// <inheritdoc />
    public void Subscribe(IDiceRollSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    /// <inheritdoc />
    public void Unsubscribe(IDiceRollSubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    /// <inheritdoc />
    public async Task NotifyRollAsync()
    {
        foreach (var subscriber in _subscribers.ToList())
        {
            await subscriber.RollAsync();
        }

        RaiseStateChanged();
    }

    /// <inheritdoc />
    public void RaiseStateChanged()
    {
        StateChanged?.Invoke();
    }
}

