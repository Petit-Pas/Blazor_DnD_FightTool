using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;
using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Damage.Validation;

/// <summary>
///    Validator for <see cref="DamageRollResult"/>
/// </summary>
public class DamageRollResultValidator : AbstractValidator<DamageRollResult>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DamageRollResultValidator()
    {
        RuleFor(x => x.Damage)
            .GreaterThanOrEqualTo(x => x.Dices.MinimumRoll())
            .LessThanOrEqualTo(x => x.Dices.MaximumRoll() * 2);
    }
}

