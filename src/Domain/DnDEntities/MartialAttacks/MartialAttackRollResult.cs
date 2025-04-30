using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Damage;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks;

/// <summary>
///     Holds the result of a martial attack roll
///     Target + hit & damage rolls
/// </summary>
public class MartialAttackRollResult
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="hitRoll"> empty hitRollResult </param>
    /// <param name="damageRolls"> empty damageRollResult </param>
    public MartialAttackRollResult(HitRollResult hitRoll, DamageRollResult[] damageRolls)
    {
        HitRoll = hitRoll;
        DamageRolls = damageRolls;
    }

    /// <summary>
    ///     The hitRoll, will start as a template and contain data after the roll
    /// </summary>
    public HitRollResult HitRoll { get; set; }

    /// <summary>
    ///     The damageRolls, will start as a template and contain data after the roll
    /// </summary>
    public DamageRollResult[] DamageRolls { get; set; } = [];

    /// <summary>
    ///     The unique non meaningful id of the target
    /// </summary>
    public Guid TargetId { get; set; } = Guid.Empty;
}
