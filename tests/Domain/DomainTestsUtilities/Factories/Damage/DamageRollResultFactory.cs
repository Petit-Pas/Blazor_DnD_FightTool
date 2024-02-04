using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace FightTestsUtilities.Factories.Damage;

public static class DamageRollResultFactory
{
    public static DamageRollResult BuildRolledDice(DamageTypeEnum? damageType = null, int? damage = null)
    {
        return new DamageRollResult(default!, damageType ?? DamageTypeEnum.Fire) 
        { 
            Damage = damage ?? 10 
        };
    }

    public static DamageRollResult Build(DiceThrowTemplate? template = null, DamageTypeEnum? damageType = null, int? damage = null, SituationalDamageModifierEnum? situationalDamageModifier = null)
    {
        var result = new DamageRollResult(template ?? new DiceThrowTemplate("2d6"), damageType ?? DamageTypeEnum.Fire, situationalDamageModifier ?? SituationalDamageModifierEnum.Normal);

        result.Damage = damage ?? result.Dices.MinimumRoll();
        return result;
    }

    public static DamageRollResult[] BuildCollection()
    {
        return new DamageRollResult[] { Build() };
    }
}
