using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DnDFightTool.Domain.DnDEntities.Damage;

public class DamageRollResult
{
    // Model is simplified for now, this represents the damage rolled
    public int Damage { get; set; }

    public DiceThrowTemplate Dices { get; set; }

    public DamageTypeEnum DamageType { get; set; } = DamageTypeEnum.Fire;

    public SituationalDamageModifierEnum SuccessfulSaveModifier { get; set; } = SituationalDamageModifierEnum.Normal;
}
