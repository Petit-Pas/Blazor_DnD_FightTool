using DnDFightTool.Domain.Fight.Characters;

namespace DnDFightTool.Domain.Fight.TurnTracking;

/// <summary>
///     State container for turn and round tracking within a fight.
///     Singleton, fight-scoped. Commands drive all transitions via setters.
/// </summary>
public interface ICombatTurnService
{
    /// <summary>
    ///     <c>true</c> when <see cref="CurrentTurnFighter"/> is not null (combat has started).
    /// </summary>
    bool IsStarted { get; }

    /// <summary>
    ///     1-based round counter. 0 until started.
    /// </summary>
    int CurrentRound { get; }

    /// <summary>
    ///     The fighter whose turn it currently is. <c>null</c> until started.
    /// </summary>
    IFightingCharacter? CurrentTurnFighter { get; }

    /// <summary>
    ///     Ordered list of fighters (descending initiative, tie-break by insertion order).
    ///     Computed once during <see cref="Initialize"/>.
    /// </summary>
    IReadOnlyList<IFightingCharacter> TurnOrder { get; }

    /// <summary>
    ///     Sorts fighters by <see cref="IFightingCharacter.InitiativeSortKey"/> into <see cref="TurnOrder"/>. Resets state.
    /// </summary>
    /// <param name="fighters">The fighters to sort.</param>
    void Initialize(IEnumerable<IFightingCharacter> fighters);

    /// <summary>
    ///     Returns the next fighter in <see cref="TurnOrder"/> after the current one, wrapping to index 0 at the end.
    ///     If not started, returns the first fighter. Does <b>not</b> mutate state.
    /// </summary>
    IFightingCharacter GetNextFighter();

    /// <summary>
    ///     Returns <c>true</c> if the current fighter is the last in <see cref="TurnOrder"/>.
    /// </summary>
    bool IsLastTurnOfRound();

    /// <summary>
    ///     Sets the current turn fighter by ID lookup in <see cref="TurnOrder"/>.
    ///     Pass <c>null</c> to clear. Fires <see cref="OnChanged"/>.
    /// </summary>
    /// <param name="fighterId">The fighter's ID, or <c>null</c> to clear.</param>
    void SetCurrentTurnFighter(Guid? fighterId);

    /// <summary>
    ///     Sets the round counter. Fires <see cref="OnChanged"/>.
    /// </summary>
    /// <param name="round">The round number to set.</param>
    void SetCurrentRound(int round);

    /// <summary>
    ///     Fired after <see cref="SetCurrentTurnFighter"/>, <see cref="SetCurrentRound"/>, and <see cref="Initialize"/>.
    /// </summary>
    event Action? OnChanged;
}
