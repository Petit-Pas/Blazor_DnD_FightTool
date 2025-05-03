using System.Diagnostics;
using System.Text.RegularExpressions;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Saves;

/// <summary>
///     This describes the difficulty class of a character
///     It has to be different from other DC because it cannot reference the "DC" wildcard, 
///         unlike the DC of a spell for instance, which can use "DC" to reference the DC of the character, so this one.
/// </summary>
[DebuggerDisplay("{Expression}")]
public partial class CharacterDifficultyClassTemplate : IHashable
{
    /// <summary>
    ///     Ctor for empty expression
    ///     Used for serialization
    /// </summary>
    public CharacterDifficultyClassTemplate()
    {
    }

    /// <summary>
    ///     Ctor that receives the xpression
    /// </summary>
    /// <param name="expression"></param>
    public CharacterDifficultyClassTemplate(string expression)
    {
        Expression = expression;
    }

    /// <summary>
    ///     The regex that validates and parses the expression
    /// </summary>
    internal readonly static Regex _regex = CharacterDifficultyClassTemplateRegex();

    /// <summary>
    ///     Accessors for the expression
    ///     Get builds it from the internal state
    ///     Set parses it and sets the internal state
    /// </summary>
    public string Expression { get => CreateExpression(); set => ParseExpression(value); }

    /// <summary>
    ///     Internal method to parse the expression
    ///     Doing this instead of keeping the original expression makes sure that we "clean" it up everytime
    ///         Ex: 8+2+MAS will automatically become 10+MAS
    /// </summary>
    /// <param name="expression"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void ParseExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            _wildcards = [];
            _staticModifier = 0;
            return;
        }

        var rgxMatch = _regex.Match(expression);
        if (!rgxMatch.Success)
        {
            throw new InvalidOperationException($"The expression {expression} is not a valid expression to describe a character DC.");
        }

        var wildcards = new List<Wildcard>();
        var staticModifier = 0;

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

        if (_staticModifier != 0)
        {
            expression += _staticModifier;
        }

        if (_wildcards.Length != 0)
        {
            expression += string.Join("", _wildcards.Select(x => $"+{x.Token}"));
            
            if (expression[0] == '+')
            {
                expression = expression[1..];
            }
        }

        return expression;
    }

    /// <summary>
    ///     Internal state for the wildcards to add to the roll
    /// </summary>
    internal Wildcard[] _wildcards = [];

    /// <summary>
    ///     Internal state for the static modifier to add to the roll
    /// </summary>
    internal int _staticModifier = 0;

    /// <summary>
    ///     Gets the DC of the caster of an effect (resolved wildcards + static modifier)
    /// </summary>
    /// <param name="caster"> The character for which DC is requested </param>
    /// <returns></returns>
    public int GetDc(Character caster)
    {
        return new ScoreModifier(_staticModifier + _wildcards.Sum(x => x.Resolve(caster).Modifier));
    }

    [GeneratedRegex(@"^((?:[0-9]+)|(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)))((?:(?:\+|\-)(?:[0-9]+))|(?:\+(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS))))*$", RegexOptions.IgnoreCase, "fr-BE")]
    private static partial Regex CharacterDifficultyClassTemplateRegex();
}
