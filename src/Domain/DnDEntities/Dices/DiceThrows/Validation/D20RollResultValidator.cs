using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;

/// <summary>
///     Validator for <see cref="D20BaseRollResult" />
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class D20RollResultValidator<T> : AbstractValidator<T>
    where T : D20BaseRollResult
{
    /// <summary>
    ///    Ctor
    /// </summary>
    public D20RollResultValidator()
    {
        RuleFor(x => x.Result)
            .InclusiveBetween(1, 20);
    }
}
