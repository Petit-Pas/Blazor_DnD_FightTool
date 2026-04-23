using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.Rolls;
using UndoableMediator.Queries;

namespace DnDFightTool.Business.DnDQueries.MartialAttackQueries;

/// <summary>
///    Query to get the result of a martial attack roll
/// </summary>
public class MartialAttackRollResultQuery : QueryBase<MartialAttackRollResult>
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="casterId"></param>
    /// <param name="attackId"></param>
    public MartialAttackRollResultQuery(Guid casterId, Guid attackId)
    {
        CasterId = casterId;
        AttackId = attackId;
    }

    /// <summary>
    ///     The id of the caster performing the attack
    /// </summary>
    public Guid CasterId { get; }

    /// <summary>
    ///     The id of the martial attack template to roll
    /// </summary>
    public Guid AttackId { get; }
}
