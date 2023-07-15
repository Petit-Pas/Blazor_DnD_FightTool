using DnDFightTool.Domain.DnDEntities.AttackRolls.ArmorClasses.Validation;
using DnDFightTool.Domain.DnDEntities.HitPoint.Validation;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Characters.Validation;

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