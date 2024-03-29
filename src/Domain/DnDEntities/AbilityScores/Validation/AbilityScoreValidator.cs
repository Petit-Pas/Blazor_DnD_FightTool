﻿using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.AbilityScores.Validation;

/// <summary>
///     Validator for <see cref="AbilityScore"/>
/// </summary>
public class AbilityScoreValidator : AbstractValidator<AbilityScore>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public AbilityScoreValidator()
    {
        RuleFor(x => x.Score).GreaterThanOrEqualTo(1).WithMessage(x => $"{x.Ability.ShortName()} cannot be lower than 1.");
        RuleFor(x => x.Score).LessThanOrEqualTo(30).WithMessage(x => $"{x.Ability.ShortName()} cannot be above 30.");
    }
}