using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDFightTool.Business.DnDQueries.MartialAttackQueries;

/// <summary>
///     Query to get the roll result of a martial attack
/// </summary>
public class MartialAttackRollResultQuery : CasterQueryBase<MartialAttackRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="martialAttackTemplateId"></param>
    /// <param name="casterId"></param>
    public MartialAttackRollResultQuery(Guid martialAttackTemplateId, Guid casterId) : base(casterId)
    {
        MartialAttackTemplateId = martialAttackTemplateId;
    }

    /// <summary>
    ///     Id of the martial attack to execute
    /// </summary>
    public Guid MartialAttackTemplateId { get; }
}
