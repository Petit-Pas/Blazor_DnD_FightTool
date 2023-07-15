using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Damage;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks;

public class MartialAttackRollResult
{
    public MartialAttackRollResult(HitRollResult hitRoll, DamageRollResult[] damageRolls)
    {
        HitRoll = hitRoll;
        DamageRolls = damageRolls;
    }

    public HitRollResult HitRoll { get; set; }
    public DamageRollResult[] DamageRolls { get; set; } = Array.Empty<DamageRollResult>();

    public Guid TargetId { get; set; } = Guid.Empty;
}
