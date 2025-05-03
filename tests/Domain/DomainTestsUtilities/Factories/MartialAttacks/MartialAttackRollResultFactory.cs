using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DomainTestsUtilities.Factories.Damage;
using DomainTestsUtilities.Factories.Dices.DiceThrows;

namespace DomainTestsUtilities.Factories.MartialAttacks;

public class MartialAttackRollResultFactory
{
    public static MartialAttackRollResult Build(HitRollResult? hitRollResult = null, DamageRollResult[]? damageRollResult = null, Guid? targetId = null)
    {
        return new MartialAttackRollResult(hitRollResult ?? HitRollResultFactory.Build(), damageRollResult ?? DamageRollResultFactory.BuildCollection())
        {
            TargetId = targetId ?? Guid.NewGuid()
        };
    }
}
