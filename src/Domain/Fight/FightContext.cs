using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight.Characters;
using DnDFightTool.Infrastructure.Mapping;
using Microsoft.Extensions.Logging;

namespace DnDFightTool.Domain.Fight;

/// <summary>
///    The context of a single fight
/// </summary>
public class FightContext : IFightContext
{
    private readonly ILogger<FightContext> _log;
    private readonly IMapper _mapper;

    // All fighters
    private readonly Dictionary<Guid, IFightingCharacter> _fighters = [];
    // Allows to increase counter per monsters when adding multiple copies of the same template
    private readonly Dictionary<Guid, int> _monsterCountByOriginalId = [];
    // Allows to easily undo the removal of a fighter from a fight
    private readonly Dictionary<Guid, IFightingCharacter> _removedFighters = [];

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="log"></param>
    /// <param name="mapper"></param>
    public FightContext(ILogger<FightContext> log, IMapper mapper)
    {
        _log = log;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public IFightingCharacter? this[Guid id]
    {
        get 
        { 
            try 
            { 
                return _fighters[id]; 
            } 
            catch (KeyNotFoundException) 
            { 
                return null;
            } 
        }
    }

    /// <inheritdoc/>
    public IEnumerable<IFightingCharacter> Fighters => _fighters.Values;

    /// <inheritdoc/>
    public event EventHandler<IFightingCharacter>? OnFighterAdded;

    /// <inheritdoc/>
    public event EventHandler<IFightingCharacter>? OnFighterRemoved;

    /// <inheritdoc/>
    public event EventHandler<Guid>? OnFighterUpdated;

    /// <inheritdoc/>
    public void NotifyFighterUpdated(Guid fighterId)
    {
        OnFighterUpdated?.Invoke(this, fighterId);
    }

    /// <inheritdoc/>
    public IFightingCharacter? Add(ICharacter character, int initiative)
    {
        FightingCharacter fighter;
        switch (character.Type)
        {
            // TODO updating the player mid fight might be creating a new instance of it, not what we want!
            case CharacterType.Player:
                fighter = new FightingCharacter(character, character.Id);
                break;
            case CharacterType.Monster:
                var count = _monsterCountByOriginalId.GetValueOrDefault(character.Id) + 1;
                _monsterCountByOriginalId[character.Id] = count;
                var monsterCopy = _mapper.Clone(character);
                monsterCopy.Name = $"{character.Name} {count}";
                fighter = new FightingCharacter(monsterCopy, character.Id);
                break;
            case CharacterType.Unknown:
            default:
                _log.LogWarning("Cannot add to fight a character of type {characterType}", character.Type);
                return null;
        }

        if (_fighters.ContainsKey(fighter.Id))
        {
            throw new InvalidOperationException($"Cannot add fighter with id {fighter.Id}: already in fight. This should never happen.");
        }
        fighter.InitiativeRoll = initiative;
        _fighters[fighter.Id] = fighter;

        OnFighterAdded?.Invoke(this, fighter);
        return fighter;
    }

    /// <inheritdoc/>
    public void Restore(Guid fighterId)
    {
        if (!_removedFighters.TryGetValue(fighterId, out var fighter))
        {
            _log.LogWarning("Cannot restore fighter with id {FighterId}: not in removed stash.", fighterId);
            return;
        }

        if (_fighters.ContainsKey(fighter.Id))
        {
            _log.LogWarning("Cannot restore fighter with id {FighterId}: already in fight.", fighterId);
            return;
        }

        _removedFighters.Remove(fighterId);
        _fighters[fighter.Id] = fighter;

        if (fighter.Type == CharacterType.Monster)
        {
            _monsterCountByOriginalId[fighter.OriginalCharacterId]++;
        }

        OnFighterAdded?.Invoke(this, fighter);
    }

    /// <inheritdoc/>
    public void Remove(IFightingCharacter fighter)
    {
        if (!_fighters.ContainsKey(fighter.Id))
        {
            _log.LogWarning("Cannot remove fighter with id {FighterId}: not in fight.", fighter.Id);
            return;
        }

        _removedFighters[fighter.Id] = fighter;
        _fighters.Remove(fighter.Id);

        if (fighter.Type == CharacterType.Monster)
        {
            _monsterCountByOriginalId[fighter.OriginalCharacterId]--;
            if (_monsterCountByOriginalId[fighter.OriginalCharacterId] <= 0)
            {
                _monsterCountByOriginalId.Remove(fighter.OriginalCharacterId);
            }
        }
        OnFighterRemoved?.Invoke(this, fighter);
    }

    /// <inheritdoc/>
    public void Remove(Guid fighterId)
    {
        var fighter = this[fighterId];
        if (fighter is not null)
        {
            Remove(fighter);
        }
    }

    /// <inheritdoc/>
    public void Update(IFightingCharacter fighter)
    {
        if (!_fighters.ContainsKey(fighter.Id))
        {
            _log.LogWarning("Cannot update fighter with id {FighterId}: not in fight.", fighter.Id);
            return;
        }
        _fighters[fighter.Id] = fighter;
    }
}