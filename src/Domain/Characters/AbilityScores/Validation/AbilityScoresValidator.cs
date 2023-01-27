﻿using System.Data;
using FluentValidation;

namespace Characters.AbilityScores.Validation;

public class AbilityScoresValidator : AbstractValidator<AbilityScoresCollection>
{
    public AbilityScoresValidator()
    {
        RuleForEach(x => x).SetValidator(new AbilityScoreValidator());
        RuleFor(x => x.MasteryBonus)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(10);
    }
}