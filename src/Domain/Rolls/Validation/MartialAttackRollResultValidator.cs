using FluentValidation;

namespace DnDFightTool.Domain.Rolls.Validation;

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
