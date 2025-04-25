using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.HitPoint.Validation;

/// <summary>
///     Validator for <see cref="HitPoints" />
/// </summary>
public class HitPointsValidator : AbstractValidator<HitPoints>
{
    /// <summary>
    ///     Since the usual comportment of a validator is to be stateless, we can use a singleton
    ///     This avoids for the need of injection or newing up when setting this validator as a sub validator
    /// </summary>
    public readonly static HitPointsValidator Instance = new();

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
