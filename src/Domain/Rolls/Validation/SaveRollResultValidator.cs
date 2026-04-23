using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.Rolls.Validation;

/// <summary>
///     Validator for <see cref="SaveRollResult" />
/// </summary>
public class SaveRollResultValidator : PropertyTargetedValidator<SaveRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public SaveRollResultValidator()
    {
        RuleFor(x => x.Result)
            .InclusiveBetween(1, 20);
    }
}
