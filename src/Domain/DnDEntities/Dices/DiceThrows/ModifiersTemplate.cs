using System.Text.RegularExpressions;
using DnDEntities.Characters;
using DnDEntities.Dices.Modifiers;

namespace DnDEntities.Dices.DiceThrows;


/// <summary>
///     This describes a modifier to apply to a dice, since it does not contain the dice expression, its only for static modifiers and wildcards
///     such as 2+WIS
/// </summary>
public class ModifiersTemplate
{
    public static Regex Regex = new Regex(@"^((?:-?[0-9]+)|(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)))((?:(?:\+|\-)(?:[0-9]+))|(?:\+(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS))))*$", RegexOptions.IgnoreCase);

    public string Expression { get => CreateExpression(); set => AnalyzeExpression(value); }

    private void AnalyzeExpression(string expression)
    {
        // TODO this error handling can probably be improved, same in DiceThrowTemplate
        if (string.IsNullOrEmpty(expression)) 
        {
            Wildcards = Array.Empty<Wildcard>();
            StaticModifier = 0;
            return;
        };

        var rgxMatch = Regex.Match(expression);
        if (!rgxMatch.Success)
        {
            throw new InvalidOperationException($"The expression {expression} is not a valid expression to describe a dice modifier expression.");
        }

        var staticModifier = 0;
        var wildcards = new List<Wildcard>();

        // We skip the first one because its a capture of the full expression.
        foreach (var capture in rgxMatch.Groups.Skip<Group>(1).SelectMany(x => x.Captures))
        {
            var term = capture.ToString();
            if (int.TryParse(term, out var number))
            {
                staticModifier += number;
            }
            else
            {
                var token = term[0] == '+' ? term.Substring(1) : term;
                wildcards.Add(new Wildcard(token));
            }
        }

        Wildcards = wildcards.ToArray();
        StaticModifier = staticModifier;
    }

    private string CreateExpression()
    {
        var expression = string.Empty;

        if (Wildcards.Length != 0)
        {
            expression += string.Join("", Wildcards.Select(x => $"+{x.Token}"));
        }

        if (StaticModifier != 0)
        {
            expression += $"{(StaticModifier > 0 ? "+" : "")}{StaticModifier}";
        }

        if (expression.Length > 0 && expression[0] == '+')
        {
            expression = expression[1..];
        }

        return expression;
    }

    internal Wildcard[] Wildcards = Array.Empty<Wildcard>();

    internal int StaticModifier = 0;

    public ScoreModifier GetScoreModifier(Character caster)
    {
        return new ScoreModifier(StaticModifier + Wildcards.Sum(x => x.Resolve(caster).Modifier));
    }
}
