using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Saves.Validation;

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
        RuleFor(x => x.RolledResult)
            .InclusiveBetween(1, 20);
    }
}
