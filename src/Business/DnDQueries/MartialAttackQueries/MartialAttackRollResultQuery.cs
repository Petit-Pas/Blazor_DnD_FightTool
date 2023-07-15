using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDFightTool.Business.DnDQueries.MartialAttackQueries;

public class MartialAttackRollResultQuery : CasterQueryBase<MartialAttackRollResult>
{
    public MartialAttackRollResultQuery(Guid martialAttackTemplateId, Guid casterId) : base(casterId)
    {
        MartialAttackTemplateId = martialAttackTemplateId;
    }

    public Guid MartialAttackTemplateId { get; }
}
