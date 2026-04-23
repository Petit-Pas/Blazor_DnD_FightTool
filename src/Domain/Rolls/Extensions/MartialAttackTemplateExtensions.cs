using DnDFightTool.Domain.CharacterSheet.MartialAttacks;
using DnDFightTool.Domain.CharacterSheet.Dices;

namespace DnDFightTool.Domain.Rolls.Extensions;

/// <summary>
///     Extension methods for converting martial attack templates to roll results.
/// </summary>
public static class MartialAttackTemplateExtensions
{
    /// <summary>
    ///     Creates a rollable result from a martial attack template.
    /// </summary>
    /// <param name="attackTemplate"></param>
    /// <returns></returns>
    public static MartialAttackRollResult GetRollableResult(this MartialAttackTemplate attackTemplate)
    {
        var hitRollResult = new HitRollResult(attackTemplate.ToHitModifiers);
        var damageRollResult = attackTemplate.Damages.GetRollableResult();
        
        return new MartialAttackRollResult(hitRollResult, damageRollResult);
    }
}
