using System.Xml.Linq;
using FluentValidation;

namespace Characters.Characteristics.Validation
{
    public class AbilityScoreValidator : AbstractValidator<AbilityScore>
    {
        public AbilityScoreValidator()
        {
            RuleFor(x => x.Value).GreaterThanOrEqualTo(1).WithMessage(x => $"{x.ShortName} cannot be lower than 1.");
            RuleFor(x => x.Value).LessThanOrEqualTo(30).WithMessage(x => $"{x.ShortName} cannot be above 30.");
        }
    }
}
