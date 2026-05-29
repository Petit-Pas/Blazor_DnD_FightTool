using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight.Characters;

namespace DnDFightTool.Domain.Fight;

/// <summary>
///    The context of a single fight
/// </summary>
public interface IFightContext
{
    /// <summary>
    ///     Generic method to add a player or a monster to a fight.
    ///     Returns the created <see cref="IFightingCharacter"/>, or <c>null</c> if the character type is unsupported
    ///     or the fighter was already present.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="initiative">The initiative roll to assign to the new fighter.</param>
    IFightingCharacter? Add(ICharacter character, int initiative);

    /// <summary>
    ///     Re-insert a fighter previously removed via <see cref="Remove"/> from the internal stash (used by remove-undo).
    ///     Re-increments the per-template counter for monsters and fires <see cref="OnFighterAdded"/>.
    /// </summary>
    /// <param name="fighterId">The <see cref="IFightingCharacter.Id"/> of the fighter to restore.</param>
    void Restore(Guid fighterId);

    /// <summary>
    ///     Remove a character from the fight
    /// </summary>
    /// <param name="character"></param>
    void Remove(IFightingCharacter character);

    /// <summary>
    ///     Remove a fighter from the fight by id. No-op if the id is not found.
    /// </summary>
    /// <param name="fighterId">The <see cref="IFightingCharacter.Id"/> of the fighter to remove.</param>
    void Remove(Guid fighterId);

    /// <summary>
    ///     Updates a character already in fight
    /// </summary>
    /// <param name="character"></param>
    void Update(IFightingCharacter character);

    /// <summary>
    ///     All Fighters
    /// </summary>
    IEnumerable<IFightingCharacter> Fighters { get; }

    /// <summary>
    ///     An event that is fired when a fighter is added to the fight (via <see cref="Add"/> or <see cref="Restore"/>).
    /// </summary>
    event EventHandler<IFightingCharacter> OnFighterAdded;

    /// <summary>
    ///     An event that is fired when a fighter is removed from the fight.
    /// </summary>
    event EventHandler<IFightingCharacter> OnFighterRemoved;

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
    IFightingCharacter? this[Guid id] { get; }
}
