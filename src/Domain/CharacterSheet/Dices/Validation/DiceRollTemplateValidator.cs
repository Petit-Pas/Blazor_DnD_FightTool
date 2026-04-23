using DnDFightTool.Infrastructure.Extensions;
using FluentValidation;

namespace DnDFightTool.Domain.CharacterSheet.Dices.Validation;

/// <summary>
///     Validator for <see cref="DiceRollTemplate" />
/// </summary>
public class DiceRollTemplateValidator : PropertyTargetedValidator<DiceRollTemplate>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public DiceRollTemplateValidator()
    {
        RuleFor(x => x.Expression)
            .NotEmpty()
            .Matches(DiceRollTemplate._regex)
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
        var regexEvaluation = DiceRollTemplate._regex.Match(expression);
        return regexEvaluation.Success;
    }
}
