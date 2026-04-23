using Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Dices.Validation;

/// <summary>
///     Validator for <see cref="IDiceRollResult" />.
///     Validates that Result is within the [Min, Max] range defined by the instance.
/// </summary>
public class DiceRollResultValidator : PropertyTargetedValidator<IDiceRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DiceRollResultValidator()
    {
        RuleFor(x => x.Result)
            .GreaterThanOrEqualTo(x => x.Min)
            .WithMessage(x => $"Must be at least {x.Min}")
            .LessThanOrEqualTo(x => x.Max)
            .WithMessage(x => $"Must be at most {x.Max}");
    }
}
