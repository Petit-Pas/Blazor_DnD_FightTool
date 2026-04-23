using DnDFightTool.Domain.CharacterSheet.Dices.Validation;
using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.Saves.Validation;

/// <summary>
///     Validator for DifficultyClass
/// </summary>
public class DifficultyClassValidator : PropertyTargetedValidator<DifficultyClassTemplate>
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
