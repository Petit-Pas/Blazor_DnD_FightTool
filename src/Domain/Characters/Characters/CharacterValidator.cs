using Fight.AttackRolls.ArmorClasses;
using FluentValidation;

namespace Characters.Characters;

public class CharacterValidator : AbstractValidator<Character>
{
    public CharacterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        //RuleFor(x => x.ArmorClass)
        //    .SetValidator(new ArmorClassValidator());
    }
}