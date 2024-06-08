using System.Diagnostics;
using System.Text.RegularExpressions;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

/// <summary>
///     This describes a full roll expression, that can contain dices, static modifiers and wildcards.
///     So it can be represented as for instance: 1d8+2d4+3+STR
/// </summary>
[DebuggerDisplay("{Expression}")]
public class DiceThrowTemplate : IHashable
{
    /// <summary>
    ///     Ctor for empty expression
    ///     As well as for serializer
    /// </summary>
    public DiceThrowTemplate()
    {
    }

    /// <summary>
    ///     Ctor that receives the expression
    /// </summary>
    /// <param name="expression"></param>
    public DiceThrowTemplate(string expression)
    {
        Expression = expression;
    }

    /// <summary>
    ///     The regex that validates and parses the expression
    /// </summary>
    internal static readonly Regex Regex = new Regex(@"^((?:[0-9]+)|(?:[0-9]+d[0-9]+)|(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)|(?:DC)))((?:(?:\+|\-)(?:[0-9]+))|(?:(?:\+|\-)(?:[0-9]+d[0-9]+))|(?:\+(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)|(?:DC))))*$", RegexOptions.IgnoreCase);

    /// <summary>
    ///     Accessors for the expression
    ///     Get builds it from the internal state
    ///     Set parses it and sets the internal state
    /// </summary>
    public string Expression { get => CreateExpression(); set => ParseExpression(value); }

    /// <summary>
    ///     Internal method to parse the expression
    /// </summary>
    /// <param name="expression"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void ParseExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            _dicesToRoll = Array.Empty<Dices>();
            _wildcards = Array.Empty<Wildcard>();
            _staticModifier = 0;
            return;
        }

        var rgxMatch = Regex.Match(expression);
        if (!rgxMatch.Success) 
        {
            throw new InvalidOperationException($"The expression {expression} is not a valid expression to describe a dice throw.");
        }

        var dices = new List<Dices>();
        var wildcards = new List<Wildcard>();
        var staticModifier = 0;

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

        _dicesToRoll = dices.Where(x => x.Amount != 0).OrderByDescending(x => x.Value).ToArray();
        _wildcards = wildcards.ToArray();
        _staticModifier = staticModifier;
    }

    /// <summary>
    ///     Private method to add dice expression to a dice list
    /// </summary>
    /// <param name="dices"></param>
    /// <param name="diceExpression"></param>
    private void AddDices(List<Dices> dices, string diceExpression)
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

    /// <summary>
    ///    Returns the expression that describes the dices to roll only
    /// </summary>
    /// <returns></returns>
    public string DicesToRollExpression()
    {
        var expression = string.Empty;

        if (_dicesToRoll.Length != 0)
        {
            expression += string.Join("", _dicesToRoll.Select(x => x.ToString(true)));

            if (expression[0] == '+')
            {
                expression = expression[1..];
            }
        }

        return expression;
    }

    /// <summary>
    ///     Build the full expression that describes the internal state
    /// </summary>
    /// <returns></returns>
    private string CreateExpression()
    {
        var expression = string.Empty;

        expression += DicesToRollExpression();

        if (_wildcards.Length != 0)
        {
            expression += string.Join("", _wildcards.Select(x => $"+{x.Token}"));
        }

        if (_staticModifier != 0)
        {
            expression += $"{(_staticModifier > 0 ? "+" : "")}{_staticModifier}";
        }

        if (expression[0] == '+')
        {
            expression = expression[1..];
        }

        return expression;
    }

    /// <summary>
    ///     Internal state for the dices to roll
    /// </summary>
    internal Dices[] _dicesToRoll = Array.Empty<Dices>();

    /// <summary>
    ///     Internal state for the wildcards to add to the roll
    /// </summary>
    internal Wildcard[] _wildcards = Array.Empty<Wildcard>();

    /// <summary>
    ///     Internal state for the static modifier to add to the roll
    /// </summary>
    internal int _staticModifier = 0;

    /// <summary>
    ///     Gets the score modifier for the roll (resolved wildcards + static modifier)
    /// </summary>
    /// <param name="caster"> The character for which the wildcards needs to be resolved </param>
    /// <returns></returns>
    public ScoreModifier GetScoreModifier(Character caster)
    {
        return new ScoreModifier(_staticModifier + _wildcards.Sum(x => x.Resolve(caster).Modifier));
    }

    /// <summary>
    ///     Gets the dices to roll only
    /// </summary>
    /// <returns></returns>
    public Dices[] GetDicesToRoll()
    {
        return _dicesToRoll;
    }

    /// <summary>
    ///     Gets the minimum to roll. That's without any wildcards or static modifier
    /// </summary>
    /// <returns></returns>
    public int MinimumRoll()
    {
        return _dicesToRoll.Sum(x => x.Amount);
    }

    /// <summary>
    ///     Gets the minimum rollable result, so dice + modifiers
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public int MinimumResult(Character character)
    {
        return MinimumRoll() + GetScoreModifier(character).Modifier;
    }

    /// <summary>
    ///     Get the maximum to roll. That's without any wildcards or static modifier
    /// </summary>
    /// <returns></returns>
    public int MaximumRoll()
    {
        return _dicesToRoll.Sum(x => x.MaximumRoll());
    }

    /// <summary>
    ///     Gets the maximum rollable result, so dice + modifiers
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public int MaximumResult(Character character)
    {
        return MaximumRoll() + GetScoreModifier(character);
    }
}
