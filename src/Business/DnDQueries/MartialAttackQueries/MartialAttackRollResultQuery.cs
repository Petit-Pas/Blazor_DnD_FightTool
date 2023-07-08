using Fight.MartialAttacks;
using UndoableMediator.Queries;

namespace DnDQueries.MartialAttackQueries;

public class MartialAttackRollResultQuery : CasterQueryBase<MartialAttackRollResult>
{
    public MartialAttackRollResultQuery(Guid martialAttackTemplateId, Guid casterId) : base(casterId)
    {
        MartialAttackTemplateId = martialAttackTemplateId;
    }

    public Guid MartialAttackTemplateId { get; }
}
