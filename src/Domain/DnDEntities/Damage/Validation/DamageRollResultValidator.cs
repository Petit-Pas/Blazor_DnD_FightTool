using Fight.Damage;
using FluentValidation;
using FluentValidation.Validators;

namespace DnDEntities.Damage.Validation;

public class DamageRollResultValidator : AbstractValidator<DamageRollResult>
{
    public DamageRollResultValidator()
    {
        RuleFor(x => x.Damage)
            .GreaterThanOrEqualTo(x => x.Dices.MinimumRoll())
            .LessThanOrEqualTo(x => x.Dices.MaximumRoll() * 2);
    }

   
}

public static class Test
{
    public static IRuleBuilderOptions<T, TProperty> InclusiveBetween<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, TProperty from, TProperty to) where TProperty : IComparable<TProperty>, IComparable
    {
        return ruleBuilder.SetValidator(RangeValidatorFactory.CreateInclusiveBetween<T, TProperty>(from, to));
    }
}
