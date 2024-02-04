using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Saves.Validation;

/// <summary>
///     Validator for DifficultyClass
/// </summary>
public class DifficultyClassValidator : AbstractValidator<DifficultyClass>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DifficultyClassValidator()
    {
        RuleFor(x => x.DifficultyClassExpression)
            .SetValidator(new ModifiersTemplateValidator());
    }
}
