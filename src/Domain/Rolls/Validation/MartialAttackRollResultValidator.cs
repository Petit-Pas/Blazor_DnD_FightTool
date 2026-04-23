using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.Rolls.Validation;

/// <summary>
///     Validator for <see cref="MartialAttackRollResult" />
/// </summary>
public class MartialAttackRollResultValidator : PropertyTargetedValidator<MartialAttackRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public MartialAttackRollResultValidator(
        IValidator<HitRollResult> hitRollResultValidator,
        IValidator<DamageRollResult> damageRollResultValidator)
    {
        RuleFor(x => x.TargetId)
            .NotEmpty();

        RuleFor(x => x.HitRoll)
            .SetValidator(hitRollResultValidator);

        RuleForEach(x => x.DamageRolls)
            .SetValidator(damageRollResultValidator);
    }
}
