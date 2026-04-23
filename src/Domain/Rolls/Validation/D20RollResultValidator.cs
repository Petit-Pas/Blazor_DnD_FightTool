using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.Rolls.Validation;

/// <summary>
///     Validator for any <see cref="D20BaseRollResult" />.
///     Validates that the result is between 1 and 20.
/// </summary>
public class D20RollResultValidator : PropertyTargetedValidator<D20BaseRollResult>
{
    /// <summary>
    ///    Ctor
    /// </summary>
    public D20RollResultValidator()
    {
        RuleFor(x => x.Result)
            .InclusiveBetween(1, 20);
    }
}
