using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight.Characters;
using Mapping;
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
    public FightingCharacter? ActiveFighter { get; private set; }
    /// <inheritdoc/>
    public event EventHandler<FightingCharacter?>? OnActiveFighterChanged;

    /// <inheritdoc/>
    public event EventHandler<FightingCharacter>? OnFighterRemoved;

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
                var monsterCopy = _mapper.Clone(character);
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

    /// <inheritdoc/>
    public void SetActiveFighter(Guid id)
    {
        if (ActiveFighter?.Id == id)
        {
            return;
        }

        if (_fighters.TryGetValue(id, out var fighter))
        {
            ActiveFighter = fighter;
            OnActiveFighterChanged?.Invoke(this, ActiveFighter);
        }
        else
        {
            // TODO warning
        }
    }
}