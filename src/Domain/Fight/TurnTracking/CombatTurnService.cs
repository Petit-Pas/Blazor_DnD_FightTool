using DnDFightTool.Domain.Fight.Characters;

namespace DnDFightTool.Domain.Fight.TurnTracking;

/// <summary>
///     In-memory implementation of <see cref="ICombatTurnService"/>.
///     Singleton, fight-scoped. Pure state container — commands drive all transitions.
/// </summary>
public class CombatTurnService : ICombatTurnService
{
    private readonly List<FightingCharacter> _turnOrder = [];
    private int _currentIndex = -1;
    private int _currentRound;

    /// <inheritdoc />
    public bool IsStarted => _currentIndex >= 0;

    /// <inheritdoc />
    public int CurrentRound => _currentRound;

    /// <inheritdoc />
    public FightingCharacter? CurrentTurnFighter => _currentIndex >= 0 ? _turnOrder[_currentIndex] : null;

    /// <inheritdoc />
    public IReadOnlyList<FightingCharacter> TurnOrder => _turnOrder;

    /// <inheritdoc />
    public event Action? OnChanged;

    /// <inheritdoc />
    public void Initialize(IEnumerable<FightingCharacter> fighters)
    {
        _turnOrder.Clear();
        _turnOrder.AddRange(fighters.OrderBy(FightingCharacter.InitiativeSortKey));
        _currentIndex = -1;
        _currentRound = 0;
        OnChanged?.Invoke();
    }

    /// <inheritdoc />
    public FightingCharacter GetNextFighter()
    {
        if (_turnOrder.Count == 0)
        {
            throw new InvalidOperationException("Cannot get next fighter: TurnOrder is empty. Call Initialize first.");
        }

        if (_currentIndex == -1)
        {
            return _turnOrder[0];
        }

        return _turnOrder[(_currentIndex + 1) % _turnOrder.Count];
    }

    /// <inheritdoc />
    public bool IsLastTurnOfRound()
    {
        return _currentIndex == _turnOrder.Count - 1;
    }

    /// <inheritdoc />
    public void SetCurrentTurnFighter(Guid? fighterId)
    {
        if (fighterId is null)
        {
            _currentIndex = -1;
        }
        else
        {
            var index = _turnOrder.FindIndex(f => f.Id == fighterId.Value);
            if (index < 0)
            {
                throw new InvalidOperationException($"Fighter with ID {fighterId} not found in TurnOrder.");
            }

            _currentIndex = index;
        }

        OnChanged?.Invoke();
    }

    /// <inheritdoc />
    public void SetCurrentRound(int round)
    {
        _currentRound = round;
        OnChanged?.Invoke();
    }
}
