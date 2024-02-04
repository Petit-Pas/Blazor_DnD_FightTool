using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///    Represents the result of a damage roll
///    // TODO Model is simplified for now, this represents the damage rolled, but we don't have the details of the roll
/// </summary>
public class DamageRollResult
{
    public DamageRollResult(DiceThrowTemplate dices, DamageTypeEnum damageType, SituationalDamageModifierEnum successfulSaveModifier = SituationalDamageModifierEnum.Normal)
    {
        Dices = dices;
        DamageType = damageType;
        SuccessfulSaveModifier = successfulSaveModifier;
    }

    /// <summary>
    ///     Represents the damage rolled
    /// </summary>
    public int Damage { get; set; }

    /// <summary>
    ///     Represents the dices that were rolled (not the result of the roll)
    /// </summary>
    public DiceThrowTemplate Dices { get; set; }

    /// <summary>
    ///     The type of the damage for this specific damage roll.
    ///     An attack that deals multiple types of damage will have multiple damage rolls.
    /// </summary>
    public DamageTypeEnum DamageType { get; set; } = DamageTypeEnum.Fire;

    /// <summary>
    ///     
    /// </summary>
    public SituationalDamageModifierEnum SuccessfulSaveModifier { get; set; } = SituationalDamageModifierEnum.Normal;
}
