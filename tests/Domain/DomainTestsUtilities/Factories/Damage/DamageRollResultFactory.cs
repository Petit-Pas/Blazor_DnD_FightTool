using DnDFightTool.Domain.DnDEntities.Damage;

namespace FightTestsUtilities.Factories.Damage;

public static class DamageRollResultFactory
{
    public static DamageRollResult Build(DamageTypeEnum damageType, int damage)
    {
        return new DamageRollResult()
        {
            Damage = damage,
            DamageType = damageType,
        };
    }
}
