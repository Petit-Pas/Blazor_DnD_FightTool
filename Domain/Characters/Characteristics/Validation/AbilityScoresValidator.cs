using System.Data;
using FluentValidation;

namespace Characters.Characteristics.Validation
{
    public class AbilityScoresValidator : AbstractValidator<AbilityScores>
    {
        public AbilityScoresValidator()
        {
            RuleForEach(x => x).SetValidator(new AbilityScoreValidator());
            RuleFor(x => x.MasteryBonus)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(10);
        }
    }
}
