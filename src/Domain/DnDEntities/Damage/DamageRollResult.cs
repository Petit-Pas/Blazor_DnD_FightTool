using DnDFightTool.Domain.DnDEntities.Dices;

namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///    Represents the result of a damage roll
///    // TODO Model is simplified for now, this represents the damage rolled, but we don't have the details of the roll
/// </summary>
public class DamageRollResult : IDiceRollResult, IRollRangeChangedNotifier
{
    public DamageRollResult(DiceRollTemplate dices, DamageTypeEnum damageType, SituationalDamageModifierEnum successfulSaveModifier = SituationalDamageModifierEnum.Normal)
    {
        Dices = dices;
        DamageType = damageType;
        SuccessfulSaveModifier = successfulSaveModifier;
    }

    /// <summary>
    ///     Represents the damage rolled
    /// </summary>
    public int Damage { get; set; }

    /// <inheritdoc />
    public int Result { get => Damage; set => Damage = value; }

    /// <inheritdoc />
    public int Min => IsCritical ? Dices.MinimumRoll() * 2 : Dices.MinimumRoll();

    /// <inheritdoc />
    public int Max => IsCritical ? Dices.MaximumRoll() * 2 : Dices.MaximumRoll();

    /// <summary>
    ///     Whether this damage roll is part of a critical hit, doubling the minimum and maximum allowed values.
    ///     Fires <see cref="RangeChanged"/> when the value changes.
    /// </summary>
    public bool IsCritical
    {
        get => _isCritical;
        set
        {
            if (_isCritical != value)
            {
                _isCritical = value;
                RangeChanged?.Invoke();
            }
        }
    }

    private bool _isCritical;

    /// <inheritdoc />
    public event Action? RangeChanged;

    /// <summary>
    ///     Represents the dices that were rolled (not the result of the roll)
    /// </summary>
    public DiceRollTemplate Dices { get; set; }

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
