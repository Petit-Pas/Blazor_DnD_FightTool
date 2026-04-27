using DnDFightTool.Domain.CharacterSheet.Characters;
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
