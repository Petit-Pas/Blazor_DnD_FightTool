using DnDEntities.Characters;
using System.Collections;
using Fight.Characters;
using Microsoft.Extensions.Logging;

namespace Fight;

public class FightContext : IFightContext
{
    public FightContext(ILogger<FightContext> log, ICharacterRepository characterRepository)
    {
        _log = log;
        _characterRepository = characterRepository;
    }


    private readonly ILogger<FightContext> _log;

    /// <summary>
    ///     Hosts lightweight representations of all the characters in the fight
    /// </summary>
    private readonly List<FightingCharacter> _fighters = new();
    
    /// <summary>
    ///     Since the players (or NPCs) are unique, we keep their reference in the character repository
    /// </summary>
    private readonly ICharacterRepository _characterRepository;

    /// <summary>
    ///     Since the monsters are not unique, they are just a copy of a template, we store the list as a copy here. 
    /// </summary>
    private readonly List<Character> _monstersInFight = new ();

    public void AddToFight(Character character)
    {
        switch (character.Type)
        {
            case CharacterType.Player:
                _fighters.Add(new FightingCharacter(character));
                break;
            case CharacterType.Monster:
                var monsterCopy = character.Duplicate();
                _monstersInFight.Add(monsterCopy);
                _fighters.Add(new FightingCharacter(monsterCopy));
                break;
            case CharacterType.Unknown:
            default:
                _log.LogWarning($"Cannot add to fight a character of type {character.Type}");
                break;
        }
    }

    public void SetFightingCharacter(FightingCharacter fightingCharacter)
    {
        if (!_fighters.Contains(fightingCharacter))
        {
            // TODO Warn
            return;
        }

        if (MovingFightingCharacter != fightingCharacter)
        {
            MovingFightingCharacter = fightingCharacter;
            RaiseMovingCharacterChanged();
        }
    }


    public FightingCharacter? MovingFightingCharacter { get; private set; }

    private Character? GetCharacterFromFightingCharacter(FightingCharacter? fightingCharacter)
    {
        if (fightingCharacter == null)
        {
            return null;
        }

        switch (fightingCharacter.CharacterType)
        {
            case CharacterType.Player:
                return _characterRepository.GetCharacterById(fightingCharacter.CharacterId);
            case CharacterType.Monster:
                return _monstersInFight.SingleOrDefault(x => x.Id == fightingCharacter.CharacterId);
            case CharacterType.Unknown:
            default:
                throw new ArgumentOutOfRangeException($"There should not be a fighting character of type {fightingCharacter.CharacterId}");
        }
    }

    public Character? GetMovingCharacter()
    {
        return GetCharacterFromFightingCharacter(MovingFightingCharacter);
    }

    public Character? GetCharacterById(Guid id)
    {
        return GetCharacterFromFightingCharacter(_fighters.SingleOrDefault(x => x.CharacterId == id));
    }

    public event EventHandler<FightingCharacter?>? MovingCharacterChanged;

    private void RaiseMovingCharacterChanged()
    {
        MovingCharacterChanged?.Invoke(this, MovingFightingCharacter);
    }

    public IEnumerator<FightingCharacter> GetEnumerator()
    {
        return _fighters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IReadOnlyCollection<FightingCharacter> GetAllFightingCharacters()
    {
        return _fighters.AsReadOnly();
    }
}