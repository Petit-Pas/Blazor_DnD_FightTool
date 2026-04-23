using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DomainTestsUtilities.Factories.Damage;
using DomainTestsUtilities.Factories.Dices;

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
