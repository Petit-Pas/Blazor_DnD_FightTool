using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.Fight.DomainExtensions.Damage;

namespace DnDFightTool.Domain.Fight.DomainExtensions.MartialAttacks;

public static class MartialAttackTemplateExtensions
{
    public static MartialAttackRollResult GetRollableResult(this MartialAttackTemplate attackTemplate)
    {
        var hitRollResult = new HitRollResult(attackTemplate.ToHitModifiers);
        var damageRollResult = attackTemplate.Damages.GetRollableResult();
        
        return new MartialAttackRollResult(hitRollResult, damageRollResult);
    }
}
