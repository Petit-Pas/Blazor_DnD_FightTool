using DnDFightTool.Domain.CharacterSheet.Dices.Validation;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.Saves.Validation;

/// <summary>
///     Validator for DifficultyClass
/// </summary>
public class DifficultyClassValidator : AbstractValidator<DifficultyClassTemplate>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DifficultyClassValidator()
    {
        RuleFor(x => x.DifficultyClassExpression)
            .SetValidator(new DiceRollModifiersTemplateValidator());
    }
}
