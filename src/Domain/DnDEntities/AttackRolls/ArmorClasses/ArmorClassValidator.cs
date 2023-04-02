using FluentValidation;

namespace DnDEntities.AttackRolls.ArmorClasses;

public class ArmorClassValidator : AbstractValidator<ArmorClass>
{
    public ArmorClassValidator()
    {
        RuleFor(x => x.BaseArmorClass)
            .InclusiveBetween(1, 30);

        RuleFor(x => x.ShieldArmorClass)
            .GreaterThan(0);
    }
}