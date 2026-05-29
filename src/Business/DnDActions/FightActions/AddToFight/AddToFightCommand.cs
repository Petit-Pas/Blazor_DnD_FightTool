using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.FightActions.AddToFight;

/// <summary>
///     Adds a character (player or monster template) to the fight.
///     Prompts for an initiative roll, or inherits it when a monster of the same kind is already in the fight.
/// </summary>
public class AddToFightCommand : CommandBase
{
    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="sourceCharacterId">Id of the source <see cref="DnDFightTool.Domain.CharacterSheet.Characters.Character"/> template.</param>
    public AddToFightCommand(Guid sourceCharacterId)
    {
        SourceCharacterId = sourceCharacterId;
    }

    /// <summary>
    ///     The source character template id (player id or monster template id).
    /// </summary>
    public Guid SourceCharacterId { get; }

    /// <summary>
    ///     The id of the <see cref="DnDFightTool.Domain.Fight.Characters.FightingCharacter"/> created during execution. Set on Execute.
    /// </summary>
    public Guid? AddedFighterId { get; set; }

    /// <summary>
    ///     The initiative roll applied to the new fighter. Set on Execute.
    /// </summary>
    public int? InitiativeRoll { get; set; }
}
