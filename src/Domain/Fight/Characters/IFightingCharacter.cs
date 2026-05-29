using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Infrastructure.Mapping;

namespace DnDFightTool.Domain.Fight.Characters;

/// <summary>
///     A character actively participating in a fight.
/// </summary>
public interface IFightingCharacter : ICharacter
{
    /// <summary>
    ///     The id of the originating <see cref="Character"/> template.
    ///     For players, equals the character's own <see cref="ICharacter.Id"/>.
    ///     For monsters, equals the source template id (NOT the regenerated id of the cloned copy).
    /// </summary>
    Guid OriginalCharacterId { get; }

    /// <summary>
    ///     The initiative roll value for this fighter.
    /// </summary>
    int InitiativeRoll { get; set; }

    /// <summary>
    ///     Gets the actual initiative, including the dexterity modifier.
    /// </summary>
    int GetInitiativeTotal();

    /// <summary>
    ///     Creates a deep copy of this fighter, including the underlying character.
    /// </summary>
    IFightingCharacter Copy(IMapper mapper);

    /// <summary>
    ///     Sort key for initiative ordering: descending total initiative, then descending dexterity modifier as tiebreak.
    /// </summary>
    static Func<IFightingCharacter, (int, int)> InitiativeSortKey =>
        fc => (-fc.GetInitiativeTotal(), -((ICharacter)fc).GetInitiativeModifier());
}
