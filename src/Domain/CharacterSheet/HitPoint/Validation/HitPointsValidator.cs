using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.HitPoint.Validation;

/// <summary>
///     Validator for <see cref="HitPoints" />
/// </summary>
public class HitPointsValidator : PropertyTargetedValidator<HitPoints>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public HitPointsValidator()
    {
        RuleFor(x => x.MaxHps)
            .GreaterThan(0);

        RuleFor(x => x.CurrentHps)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.MaxHps).WithMessage(x => $"Cannot have more HPs than MaxHP: {x.MaxHps}");
    }
}
