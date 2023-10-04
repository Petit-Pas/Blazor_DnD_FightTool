using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Damage.Validation;

public class DamageRollResultValidator : AbstractValidator<DamageRollResult>
{
    public DamageRollResultValidator()
    {
        RuleFor(x => x.Damage)
            .GreaterThanOrEqualTo(x => x.Dices.MinimumRoll())
            .LessThanOrEqualTo(x => x.Dices.MaximumRoll() * 2);
    }
}

