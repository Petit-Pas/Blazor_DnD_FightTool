using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDQueries.FightQueries;

/// <summary>
///     Query that prompts the user for a single character's initiative roll (raw d20).
/// </summary>
public class InitiativeRollQuery : QueryBase<int>
{
    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="characterId">The id of the character whose initiative is being rolled.</param>
    public InitiativeRollQuery(Guid characterId)
    {
        CharacterId = characterId;
    }

    /// <summary>
    ///     The id of the character whose initiative is being rolled.
    ///     Used by the handler to display the dexterity modifier informationally.
    /// </summary>
    public Guid CharacterId { get; }
}
