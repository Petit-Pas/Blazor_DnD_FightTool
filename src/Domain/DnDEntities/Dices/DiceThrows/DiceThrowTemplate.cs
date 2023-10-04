using System.Text.RegularExpressions;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

/// <summary>
///     This describes a full roll expression, that can contain dices, static modifiers and wildcards.
///     So it can be represented as for instance: 1d8+2d4+3+STR
/// </summary>
public class DiceThrowTemplate
{
    public DiceThrowTemplate()
    {
    }

    public DiceThrowTemplate(string expression)
    {
        Expression = expression;
    }

    public static Regex Regex = new Regex(@"^((?:[0-9]+)|(?:[0-9]+d[0-9]+)|(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)|(?:DC)))((?:(?:\+|\-)(?:[0-9]+))|(?:(?:\+|\-)(?:[0-9]+d[0-9]+))|(?:\+(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)|(?:DC))))*$", RegexOptions.IgnoreCase);

    public string Expression { get => CreateExpression(); set => AnalyzeExpression(value); }

    private void AnalyzeExpression(string expression)
    {
        if (string.IsNullOrEmpty(expression))
        {
            DicesToRoll = Array.Empty<Dices>();
            Wildcards = Array.Empty<Wildcard>();
            StaticModifier = 0;
            return;
        }

        var rgxMatch = Regex.Match(expression);
        if (!rgxMatch.Success) 
        {
            throw new InvalidOperationException($"The expression {expression} is not a valid expression to describe a dice throw.");
        }

        var dices = new List<Dices>();
        var staticModifier = 0;
        var wildcards = new List<Wildcard>();

        // We skip the first one because its a capture of the full expression.
        foreach (var capture in rgxMatch.Groups.Skip<Group>(1).SelectMany(x => x.Captures))
        {
            var term = capture.ToString();
            if (term.Contains("d"))
            {
                AddDices(dices, term);
            }
            else if (int.TryParse(term, out var number))
            {
                staticModifier += number;
            }
            else
            {
                var token = term[0] == '+' ? term.Substring(1) : term;
                wildcards.Add(new Wildcard(token));
            }
        }

        DicesToRoll = dices.Where(x => x.Amount != 0).OrderByDescending(x => x.Value).ToArray();
        Wildcards = wildcards.ToArray();
        StaticModifier = staticModifier;
    }

    public void AddDices(List<Dices> dices, string diceExpression)
    {
        var sepIndex = diceExpression.IndexOf('d');
        var amount = int.Parse(diceExpression[..sepIndex]);
        var value = int.Parse(diceExpression[(sepIndex + 1)..]);

        var dice = dices.FirstOrDefault(x => x.Value == value);
        if (dice != null)
        {
            dice.Amount += amount;
        }
        else
        {
            dices.Add(new Dices(amount, value));
        }
    }

    public string DicesToRollExpression()
    {
        var expression = string.Empty;

        if (DicesToRoll.Length != 0)
        {
            expression += string.Join("", DicesToRoll.Select(x => x.ToString(true)));

            if (expression[0] == '+')
            {
                expression = expression[1..];
            }
        }

        return expression;
    }

    private string CreateExpression()
    {
        var expression = string.Empty;

        expression += DicesToRollExpression();

        if (Wildcards.Length != 0)
        {
            expression += string.Join("", Wildcards.Select(x => $"+{x.Token}"));
        }

        if (StaticModifier != 0)
        {
            expression += $"{(StaticModifier > 0 ? "+" : "")}{StaticModifier}";
        }

        if (expression[0] == '+')
        {
            expression = expression[1..];
        }

        return expression;
    }

    internal Dices[] DicesToRoll = Array.Empty<Dices>();

    internal Wildcard[] Wildcards = Array.Empty<Wildcard>();

    internal int StaticModifier = 0;

    public ScoreModifier GetScoreModifier(Character caster)
    {
        return new ScoreModifier(StaticModifier + Wildcards.Sum(x => x.Resolve(caster).Modifier));
    }

    public Dices[] GetDicesToRoll()
    {
        return DicesToRoll;
    }

    public int MinimumRoll()
    {
        return DicesToRoll.Sum(x => x.Amount);
    }

    public int MaximumRoll()
    {
        return DicesToRoll.Sum(x => x.MaximumRoll());
    }
}
