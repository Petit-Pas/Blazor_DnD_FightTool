using DnDFightTool.Domain.DnDEntities.MartialAttacks;

namespace DnDFightTool.Business.DnDUserInteraction.MartialAttackUserInteractions;

public record MartialAttackRollResultRequestInteraction(Guid CasterId, Guid AttackId) : UserInteractionBase<MartialAttackRollResult>
{
}
