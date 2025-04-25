using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks.Validation;

/// <summary>
///     Validator for <see cref="MartialAttackRollResult" />
/// </summary>
public class MartialAttackRollResultValidator : AbstractValidator<MartialAttackRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public MartialAttackRollResultValidator(
        AbstractValidator<HitRollResult> hitRollResultValidator,
        AbstractValidator<DamageRollResult> damageRollResultValidator)
    {
        RuleFor(x => x.TargetId)
            .NotEmpty();

        RuleFor(x => x.HitRoll)
            .SetValidator(hitRollResultValidator);

        RuleForEach(x => x.DamageRolls)
            .SetValidator(damageRollResultValidator);
    }
}
