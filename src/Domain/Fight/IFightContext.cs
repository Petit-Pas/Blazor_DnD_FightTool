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
    void AddToFight(Character character);

    /// <summary>
    ///     All Fighters
    /// </summary>
    IEnumerable<FightingCharacter> Fighters { get; }

    /// <summary>
    ///     the moving fighter is not the one whose turn it is.
    ///     Is is the player whose possible actions will be displayed (aka the one the user clicked on, or the one whose turn it is when you switch turn)
    /// </summary>
    FightingCharacter? ActiveFighter { get; }

    /// <summary>
    ///     Sets the moving fighter by id
    /// </summary>
    /// <param name="id"></param>
    void SetActiveFighter(Guid id);
    /// <summary>
    ///     Sets the moving fighter
    /// </summary>
    /// <param name="id"></param>
    void SetActiveFighter(FightingCharacter character) => SetActiveFighter(character.Id);

    /// <summary>
    ///     An event that is fired when the moving fighter changes
    /// </summary>
    event EventHandler<FightingCharacter?> ActiveFighterChanged;

    /// <summary>
    ///     Indexer to access fighters by Guid
    /// </summary>
    FightingCharacter this[Guid id] { get; set; }
}
