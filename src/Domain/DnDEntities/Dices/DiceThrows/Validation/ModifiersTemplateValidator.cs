using FluentValidation;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;

/// <summary>
///     Validator for <see cref="ModifiersTemplate" />
/// </summary>
public class ModifiersTemplateValidator : AbstractValidator<ModifiersTemplate>
{
    /// <summary>
    ///     Ctor
    /// </summary>
    public ModifiersTemplateValidator()
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
        var regexEvaluation = ModifiersTemplate.Regex.Match(expression);
        return regexEvaluation.Success;
    }
}
