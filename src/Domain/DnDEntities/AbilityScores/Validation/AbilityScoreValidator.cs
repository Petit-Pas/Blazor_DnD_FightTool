using Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.AbilityScores.Validation;

/// <summary>
///     Validator for <see cref="AbilityScore"/>
/// </summary>
public class AbilityScoreValidator : PropertyTargetedValidator<AbilityScore>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public AbilityScoreValidator()
    {
        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(1).WithMessage(x => $"{x.Ability.ShortName()} cannot be lower than 1.")
            .LessThanOrEqualTo(30).WithMessage(x => $"{x.Ability.ShortName()} cannot be above 30.");
    }
}