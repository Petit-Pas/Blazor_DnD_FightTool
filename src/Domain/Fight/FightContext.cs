using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight.Characters;
using Mapping;
using Microsoft.Extensions.Logging;

namespace DnDFightTool.Domain.Fight;

public class FightContext : IFightContext
{
    private readonly ILogger<FightContext> _log;
    private readonly IMapper _mapper;
    private readonly Dictionary<Guid, FightingCharacter> _fighters = new();

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
    public FightingCharacter this[Guid id]
    {
        get => _fighters[id];
        set => _fighters[id] = value;
    }

    /// <inheritdoc/>
    public IEnumerable<FightingCharacter> Fighters => _fighters.Values;

    /// <inheritdoc/>
    public FightingCharacter? ActiveFighter { get; private set; }
    /// <inheritdoc/>
    public event EventHandler<FightingCharacter?>? ActiveFighterChanged;

    /// <inheritdoc/>
    public void AddToFight(Character character)
    {
        FightingCharacter fighter;
        switch (character.Type)
        {
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
        _fighters[fighter.Id] = fighter;
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
            ActiveFighterChanged?.Invoke(this, ActiveFighter);
        }
        else
        {
            // TODO warning
        }
    }
}