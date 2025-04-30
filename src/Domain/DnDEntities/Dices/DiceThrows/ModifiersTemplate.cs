using System.Text.RegularExpressions;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;


/// <summary>
///     This describes a modifier to apply to a dice, since it does not contain the dice expression, its only for static modifiers and wildcards
///     such as 2+WIS
///     For full expressions containing dices too, use <see cref="DiceThrowTemplate" />
/// </summary>
public partial class ModifiersTemplate : IHashable
{
    /// <summary>
    ///     Empty ctor for no modifiers or serializer
    /// </summary>
    public ModifiersTemplate()
    {
    }

    /// <summary>
    ///     Ctor that receives the expression
    /// </summary>
    /// <param name="expression"></param>
    public ModifiersTemplate(string expression)
    {
        Expression = expression;
    }

    /// <summary>
    ///     Regex to validate and parse the expression
    /// </summary>
    internal readonly static Regex _regex = ModifiersTemplateRegex();

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
        // TODO this error handling can probably be improved, same in DiceThrowTemplate
        // Should it though? we have validators in the UI, and this is a domain object, so it should be valid
        if (string.IsNullOrWhiteSpace(expression)) 
        {
            _wildcards = [];
            _staticModifier = 0;
            return;
        };

        var rgxMatch = _regex.Match(expression);
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
                var token = term[0] == '+' ? term[1..] : term;
                wildcards.Add(new Wildcard(token));
            }
        }

        _wildcards = [.. wildcards];
        _staticModifier = staticModifier;
    }

    /// <summary>
    ///     Build the full expression that describes the internal state
    /// </summary>
    /// <returns></returns>
    private string CreateExpression()
    {
        var expression = string.Empty;

        if (_wildcards.Length != 0)
        {
            expression += string.Join("", _wildcards.Select(x => $"+{x.Token}"));
        }

        if (_staticModifier != 0)
        {
            expression += $"{(_staticModifier > 0 ? "+" : "")}{_staticModifier}";
        }

        if (expression.Length > 0 && expression[0] == '+')
        {
            expression = expression[1..];
        }

        return expression;
    }

    /// <summary>
    ///     Wildcards to apply
    /// </summary>
    internal Wildcard[] _wildcards = [];

    /// <summary>
    ///     Static modifier
    /// </summary>
    internal int _staticModifier = 0;

    /// <summary>
    ///     Gets the score modifier. Resolved wildcards + static modifier
    /// </summary>
    /// <param name="caster"> The character for which the wildcards needs to be resolved </param>
    /// <returns></returns>
    public ScoreModifier GetScoreModifier(Character caster)
    {
        return new ScoreModifier(_staticModifier + _wildcards.Sum(x => x.Resolve(caster).Modifier));
    }

    [GeneratedRegex(@"^((?:-?[0-9]+)|(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)|(?:DC)))((?:(?:\+|\-)(?:[0-9]+))|(?:\+(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)|(?:DC))))*$", RegexOptions.IgnoreCase, "fr-BE")]
    private static partial Regex ModifiersTemplateRegex();
}
