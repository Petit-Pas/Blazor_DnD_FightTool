using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Dices.Validation;

/// <summary>
///     Validator for <see cref="HitRollResult" />.
///     Validates that the d20 result is between 1 and 20.
/// </summary>
public class HitRollResultValidator : AbstractValidator<HitRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public HitRollResultValidator()
    {
        RuleFor(x => x.Result)
            .InclusiveBetween(1, 20);
    }
}
