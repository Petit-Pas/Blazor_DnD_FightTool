using FluentValidation;

namespace DnDEntities.AbilityScores.Validation;

public class AbilityScoreValidator : AbstractValidator<AbilityScore>
{
    public AbilityScoreValidator()
    {
        RuleFor(x => x.Score).GreaterThanOrEqualTo(1).WithMessage(x => $"{x.ShortName} cannot be lower than 1.");
        RuleFor(x => x.Score).LessThanOrEqualTo(30).WithMessage(x => $"{x.ShortName} cannot be above 30.");
    }
}