using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.Rolls.Validation;

/// <summary>
///    Validator for <see cref="DamageRollResult"/>
/// </summary>
public class DamageRollResultValidator : PropertyTargetedValidator<DamageRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DamageRollResultValidator()
    {
        RuleFor(x => x.Damage)
            .GreaterThanOrEqualTo(x => x.Min)
            .LessThanOrEqualTo(x => x.Max);
    }
}
