using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Dices.Validation;

/// <summary>
///     Validator for <see cref="DiceRollModifiersTemplate" />
/// </summary>
public class DiceRollModifiersTemplateValidator : AbstractValidator<DiceRollModifiersTemplate>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DiceRollModifiersTemplateValidator()
    {
        RuleFor(x => x.Expression)
            .Must(MatchRegex)
            .WithMessage("The expression is not valid, it should look like STR+2+MAS");
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
        var regexEvaluation = DiceRollModifiersTemplate._regex.Match(expression);
        return regexEvaluation.Success;
    }
}
