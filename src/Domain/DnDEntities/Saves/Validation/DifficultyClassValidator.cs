using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Saves.Validation;

public class DifficultyClassValidator : AbstractValidator<DifficultyClass>
{
    public DifficultyClassValidator()
    {
        RuleFor(x => x.Expression.DicesToRoll)
            .Empty()
            .WithMessage("Cannot have dices in the DC");
    }
}
