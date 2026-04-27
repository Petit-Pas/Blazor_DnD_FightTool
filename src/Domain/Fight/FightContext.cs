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

    private readonly Dictionary<Guid, FightingCharacter> _fighters = [];
    private readonly Dictionary<Guid, int> _monsterCountByOriginalId = [];

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
    public FightingCharacter? this[Guid id]
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
        set {
            if (value is not null)
            {
                _fighters[id] = value;
            }
            else
            {
                _fighters.Remove(id);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<FightingCharacter> Fighters => _fighters.Values;

    /// <inheritdoc/>
    public event EventHandler<FightingCharacter>? OnFighterRemoved;

    /// <inheritdoc/>
    public event EventHandler<Guid>? OnFighterUpdated;

    /// <inheritdoc/>
    public void NotifyFighterUpdated(Guid fighterId)
    {
        OnFighterUpdated?.Invoke(this, fighterId);
    }

    /// <inheritdoc/>
    public void Add(Character character)
    {
        FightingCharacter fighter;
        switch (character.Type)
        {
            // TODO updating the player mid fight might be creating a new instance of it, not what we want!
            case CharacterType.Player:
                fighter = new FightingCharacter(character);
                break;
            case CharacterType.Monster:
                var count = _monsterCountByOriginalId.GetValueOrDefault(character.Id) + 1;
                _monsterCountByOriginalId[character.Id] = count;
                var monsterCopy = _mapper.Clone(character);
                monsterCopy.Name = $"{character.Name} {count}";
                fighter = new FightingCharacter(monsterCopy);
                break;
            case CharacterType.Unknown:
            default:
                _log.LogWarning("Cannot add to fight a character of type {characterType}", character.Type);
                return;
        }
        
        if (_fighters.ContainsKey(fighter.Id))
        {
            return;
        }
        _fighters[fighter.Id] = fighter;
    }

    /// <inheritdoc/>
    public void Remove(FightingCharacter fightingCharacter)
    {
        if (_fighters.ContainsKey(fightingCharacter.Id))
        {
            _fighters.Remove(fightingCharacter.Id);
            OnFighterRemoved?.Invoke(this, fightingCharacter);
        }
        else
        {
            // TODO warn?
        }
    }

    public void Update(FightingCharacter fightingCharacter)
    {
        if (_fighters.ContainsKey(fightingCharacter.Id))
        {
            _fighters[fightingCharacter.Id] = fightingCharacter;
        }
        else
        {
            // TODO warn?
        }
    }

}