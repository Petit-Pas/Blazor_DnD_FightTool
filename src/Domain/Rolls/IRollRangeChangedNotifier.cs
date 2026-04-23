namespace DnDFightTool.Domain.Rolls;

/// <summary>
///     Implemented by <see cref="IDiceRollResult"/> types whose valid range
///     can change at runtime (e.g. <see cref="DamageRollResult"/> on a critical hit).
///     Consumers subscribe to <see cref="RangeChanged"/> to react when
///     <see cref="IDiceRollResult.Min"/> or <see cref="IDiceRollResult.Max"/> changes.
/// </summary>
public interface IRollRangeChangedNotifier
{
    /// <summary>
    ///     Raised when the roll's valid range changes.
    /// </summary>
    event Action? RangeChanged;
}
