using FluentValidation;

namespace DnDEntities.Dices.DiceThrows.Validation;

public abstract class D20RollResultValidator<T> : AbstractValidator<T>
    where T : D20BaseRollResult
{
    public D20RollResultValidator()
    {
        RuleFor(x => x.Result)
            .InclusiveBetween(1, 20);
    }
}
