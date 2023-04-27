using FluentValidation;

namespace DnDEntities.HitPoint.Validation
{
    public class HitPointsValidator : AbstractValidator<HitPoints>
    {
        public static readonly HitPointsValidator Instance = new ();

        public HitPointsValidator()
        {
            RuleFor(x => x.MaxHps)
                .GreaterThan(0);

            RuleFor(x => x.CurrentHps)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(x => x.MaxHps).WithMessage(x => $"Cannot have more HPs than MaxHP: {x.MaxHps}");
        }
    }
}
