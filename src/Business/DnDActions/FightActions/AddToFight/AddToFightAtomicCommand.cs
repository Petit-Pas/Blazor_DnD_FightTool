using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.FightActions.AddToFight;

/// <summary>
///     Atomic: adds a character to the fight and sets the initiative roll.
///     Undo removes the added fighter. Returns the added fighter's <see cref="Guid"/> id.
/// </summary>
public class AddToFightAtomicCommand : CommandBase<Guid>
{
    /// <summary>
    ///     The id of the source <see cref="DnDFightTool.Domain.CharacterSheet.Characters.ICharacter"/> template.
    ///     Resolved by the handler during execution.
    /// </summary>
    public Guid SourceCharacterId { get; }

    /// <summary>
    ///     The initiative roll to apply to the new fighter.
    /// </summary>
    public int Initiative { get; }

    /// <summary>
    ///     The id of the <see cref="DnDFightTool.Domain.Fight.Characters.IFightingCharacter"/> created during execution.
    ///     Set on Execute; used by UndoAsync to locate and remove the fighter.
    /// </summary>
    public Guid? AddedFighterId { get; set; }

    /// <summary>
    ///     Ctor.
    /// </summary>
    public AddToFightAtomicCommand(Guid sourceCharacterId, int initiative)
    {
        SourceCharacterId = sourceCharacterId;
        Initiative = initiative;
    }
}
