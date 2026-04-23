using FluentValidation;

namespace DnDFightTool.Domain.Rolls.Validation;

/// <summary>
///     Validator for <see cref="SaveRollResult" />
/// </summary>
public class SaveRollResultValidator : AbstractValidator<SaveRollResult>
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
