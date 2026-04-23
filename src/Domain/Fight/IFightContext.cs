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
    void Add(Character character);

    /// <summary>
    ///     Remove a character from the fight
    /// </summary>
    /// <param name="character"></param>
    void Remove(FightingCharacter character);

    /// <summary>
    ///     Updates a character already in fight
    /// </summary>
    /// <param name="character"></param>
    void Update(FightingCharacter character);

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
    void SetActiveFighter(FightingCharacter character)
    {
        SetActiveFighter(character.Id);
    }

    /// <summary>
    ///     An event that is fired when the moving fighter changes
    /// </summary>
    event EventHandler<FightingCharacter?> OnActiveFighterChanged;

    /// <summary>
    ///     An event that is fired when a fighter is removed from the fight.
    /// </summary>
    event EventHandler<FightingCharacter> OnFighterRemoved;

    /// <summary>
    ///     An event that is fired when a fighter's state is mutated in-place.
    /// </summary>
    event EventHandler<Guid> OnFighterUpdated;

    /// <summary>
    ///     Notifies that a fighter's state has been updated in-place.
    /// </summary>
    /// <param name="fighterId"></param>
    void NotifyFighterUpdated(Guid fighterId);

    /// <summary>
    ///     Indexer to access fighters by Guid
    /// </summary>
    FightingCharacter? this[Guid id] { get; }
}
