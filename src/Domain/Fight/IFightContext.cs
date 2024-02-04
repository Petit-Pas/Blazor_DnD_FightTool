using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight.Characters;

namespace DnDFightTool.Domain.Fight;

/// <summary>
///    The context of a single fight
/// </summary>
public interface IFightContext
{
    /// <summary>
    ///     Generic method to add a player or a monster to a fight
    /// </summary>
    /// <param name="character"></param>
    public void AddToFight(Character character);

    /// <summary>
    ///     Method set a character as currently moving.
    ///     It is the one that is doing an action atm, not especially the one whose turn it is.
    /// </summary>
    /// <param name="fighter"></param>
    public void SetMovingFighter(Fighter fighter);

    /// <summary>
    ///     the moving fighter is not the one whose turn it is.
    ///     Is is the player whose possible actions will be displayed (aka the one the user clicked on, or the one whose turn it is when you switch turn)
    /// </summary>
    Fighter? MovingFighter { get; }

    /// <summary>
    ///     An event that is fired when the moving fighter changes
    /// </summary>
    event EventHandler<Fighter?> MovingFighterChanged;

    /// <summary>
    ///     Gets all fighters in the fight
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Fighter> GetFighters();

    /// <summary>
    ///     Gets the character that is currently moving, not the fighter but the full character
    /// </summary>
    /// <returns></returns>
    public Character? GetMovingFighterCharacter();

    /// <summary>
    ///     Gets a character by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Character? GetCharacterById(Guid id);

}
