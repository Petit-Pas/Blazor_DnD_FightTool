using DnDEntities.Characters;
using Fight.Characters;

namespace Fight;

public interface IFightContext: IEnumerable<FightingCharacter>
{
    /// <summary>
    ///     Generic method to add a character or a monster to a fight
    /// </summary>
    /// <param name="character"></param>
    public void AddToFight(Character character);

    /// <summary>
    ///     Method to update the moving character
    /// </summary>
    /// <param name="fightingCharacter"></param>
    public void SetFightingCharacter(FightingCharacter fightingCharacter);

    /// <summary>
    ///     the moving fighter is not the one whose turn it is.
    ///     Is is the player whose possible actions will be displayed (aka the one the user clicked on, or the one whose turn it is when you switch turn)
    /// </summary>
    FightingCharacter? MovingFightingCharacter { get; }

    event EventHandler<FightingCharacter?> MovingCharacterChanged;

    IReadOnlyCollection<FightingCharacter> GetAllFightingCharacters();


    public Character? GetMovingCharacter();
    public Character? GetCharacterById(Guid id);

}
