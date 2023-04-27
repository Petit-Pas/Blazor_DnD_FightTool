using DnDEntities.AttackRolls.ArmorClasses.Validation;
using DnDEntities.HitPoint.Validation;
using FluentValidation;

namespace DnDEntities.Characters.Validator;

public class CharacterValidator : AbstractValidator<Character>
{
    public CharacterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.HitPoints)
            .SetValidator(HitPointsValidator.Instance);

        RuleFor(x => x.ArmorClass)
            .SetValidator(new ArmorClassValidator());
    }
}