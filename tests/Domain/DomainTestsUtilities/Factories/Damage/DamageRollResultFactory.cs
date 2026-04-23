using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.CharacterSheet.Dices;

namespace DomainTestsUtilities.Factories.Damage;

public static class DamageRollResultFactory
{
    public static DamageRollResult BuildRolledDice(DamageTypeEnum? damageType = null, int? damage = null)
    {
        return new DamageRollResult(default!, damageType ?? DamageTypeEnum.Fire)
        {
            Damage = damage ?? 10
        };
    }

    public static DamageRollResult Build(DiceRollTemplate? template = null, DamageTypeEnum? damageType = null, int? damage = null, SituationalDamageModifierEnum? situationalDamageModifier = null)
    {
        var result = new DamageRollResult(template ?? new DiceRollTemplate("2d6"), damageType ?? DamageTypeEnum.Fire, situationalDamageModifier ?? SituationalDamageModifierEnum.Normal);

        result.Damage = damage ?? result.Dices.MinimumRoll();
        return result;
    }

    public static DamageRollResult[] BuildCollection()
    {
        return [Build()];
    }
}
