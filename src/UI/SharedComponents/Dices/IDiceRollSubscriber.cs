namespace DnDFightTool.UI.SharedComponents.Dices;

/// <summary>
///     Implemented by any UI component that owns a single dice roll result
///     and participates in a <see cref="IDiceRollNotifier"/> orchestration.
/// </summary>
public interface IDiceRollSubscriber
{
    /// <summary>
    ///     Fills the owned result with a random value and refreshes the field.
    /// </summary>
    Task RollAsync();

    /// <summary>
    ///     <c>true</c> when the result has been set (non-zero).
    /// </summary>
    bool IsRolled { get; }
}
