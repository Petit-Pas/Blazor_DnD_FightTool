using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Damage.Validation;

/// <summary>
///    Validator for <see cref="DamageRollResult"/>
/// </summary>
public class DamageRollResultValidator : AbstractValidator<DamageRollResult>
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

