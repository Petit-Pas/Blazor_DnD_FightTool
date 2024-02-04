using DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses.Validation;
using DnDFightTool.Domain.DnDEntities.HitPoint.Validation;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Characters.Validation;

/// <summary>
///     Validator for <see cref="Character"/>
/// </summary>
public class CharacterValidator : AbstractValidator<Character>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public CharacterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.HitPoints)
            .SetValidator(HitPointsValidator.Instance);

        RuleFor(x => x.ArmorClass)
            .SetValidator(ArmorClassValidator.Instance);
    }
}