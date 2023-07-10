
using DnDEntities.Damage;
using DnDEntities.Dices.DiceThrows;

namespace Fight.Damage;

public class DamageRollResult
{
    // Model is simplified for now, this represents the damage rolled
    public int Damage { get; set; }

    public DiceThrowTemplate Dices { get; set; }

    public DamageTypeEnum DamageType { get; set; } = DamageTypeEnum.Fire;

    public SituationalDamageModifierEnum SuccessfullSaveModifier { get; set; } = SituationalDamageModifierEnum.Normal;
}
