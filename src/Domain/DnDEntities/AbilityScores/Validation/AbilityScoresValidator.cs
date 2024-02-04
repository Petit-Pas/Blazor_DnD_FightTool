using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.AbilityScores.Validation;

/// <summary>
///     Validator for <see cref="AbilityScoresCollection"/>
/// </summary>
public class AbilityScoresValidator : AbstractValidator<AbilityScoresCollection>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public AbilityScoresValidator()
    {
        RuleForEach(x => x).SetValidator(new AbilityScoreValidator());
        RuleFor(x => x.MasteryBonus)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(10);
    }
}