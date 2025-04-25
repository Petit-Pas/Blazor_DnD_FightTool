using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;

/// <summary>
///     Validator for <see cref="DiceThrowTemplate" />
/// </summary>
public class DiceThrowTemplateValidator : AbstractValidator<DiceThrowTemplate>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DiceThrowTemplateValidator()
    {
        RuleFor(x => x.Expression)
            .Must(MatchRegex)
            .WithMessage("The expression is not valid, it should look like 1d8+STR+2+MAS");
    }


    /// <summary>
    ///     Helper method to check that the expression is matching the regex
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static bool MatchRegex(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return true;
        }
        var regexEvaluation = DiceThrowTemplate._regex.Match(expression);
        return regexEvaluation.Success;
    }
}
