using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight.Characters;
using Microsoft.Extensions.Logging;

namespace DnDFightTool.Domain.Fight;

public class FightContext : IFightContext
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="log"></param>
    /// <param name="characterRepository"></param>
    public FightContext(ILogger<FightContext> log, ICharacterRepository characterRepository)
    {
        _log = log;
        _characterRepository = characterRepository;
    }

    private readonly ILogger<FightContext> _log;

    /// <summary>
    ///     Hosts lightweight representations of all the characters in the fight
    /// </summary>
    private readonly List<Fighter> _fighters = [];
    
    /// <summary>
    ///     Since the players (or NPCs) are unique, we keep their reference in the character repository
    /// </summary>
    private readonly ICharacterRepository _characterRepository;

    /// <summary>
    ///     Since the monsters are not unique, they are just a copy of a template, we store the list as a copy here. 
    /// </summary>
    private readonly List<Character> _monstersInFight = [];

    /// <inheritdoc/>
    public void AddToFight(Character character)
    {
        switch (character.Type)
        {
            case CharacterType.Player:
                _fighters.Add(new Fighter(character));
                break;
            case CharacterType.Monster:
                var monsterCopy = character.Duplicate();
                _monstersInFight.Add(monsterCopy);
                _fighters.Add(new Fighter(monsterCopy));
                break;
            case CharacterType.Unknown:
            default:
                _log.LogWarning($"Cannot add to fight a character of type {character.Type}");
                break;
        }
    }

    /// <inheritdoc/>
    public void SetMovingFighter(Fighter fightingCharacter)
    {
        if (!_fighters.Contains(fightingCharacter))
        {
            // TODO Warn
            return;
        }

        if (MovingFighter != fightingCharacter)
        {
            MovingFighter = fightingCharacter;
            RaiseMovingCharacterChanged();
        }
    }


    /// <inheritdoc/>
    public Fighter? MovingFighter { get; private set; }

    /// <summary>
    ///     Gets character of a fighter, either from the character repository or from the monsters in fight
    /// </summary>
    /// <param name="fighter"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private Character? GetActualCharacterFromFighters(Fighter? fighter)
    {
        if (fighter == null)
        {
            return null;
        }

        return fighter.CharacterType switch
        {
            CharacterType.Player => _characterRepository.GetCharacterById(fighter.CharacterId),
            CharacterType.Monster => _monstersInFight.SingleOrDefault(x => x.Id == fighter.CharacterId),
            _ => throw new ArgumentOutOfRangeException($"There should not be a fighting character of type {fighter.CharacterId}"),
        };
    }

    /// <inheritdoc/>
    public Character? GetMovingFighterCharacter()
    {
        return GetActualCharacterFromFighters(MovingFighter);
    }

    /// <inheritdoc/>
    public Character? GetCharacterById(Guid id)
    {
        return GetActualCharacterFromFighters(_fighters.SingleOrDefault(x => x.CharacterId == id));
    }

    /// <inheritdoc/>
    public event EventHandler<Fighter?>? MovingFighterChanged;

    /// <summary>
    ///     Raise the event
    /// </summary>
    private void RaiseMovingCharacterChanged()
    {
        MovingFighterChanged?.Invoke(this, MovingFighter);
    }

    /// <inheritdoc/>
    public IEnumerable<Fighter> GetFighters()
    {
        return _fighters;
    }
}