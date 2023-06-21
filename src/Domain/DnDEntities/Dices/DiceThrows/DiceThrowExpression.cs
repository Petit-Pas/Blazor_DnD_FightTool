using System.Text.RegularExpressions;

namespace DnDEntities.Dices.DiceThrows;

public class DiceThrowExpression
{
    public static Regex Regex = new Regex(@"^((?:[0-9]+)|(?:[0-9]+d[0-9]+)|(?:(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)))((?:(?:\+|\-)[0-9]+)|(?:(?:\+|\-)[0-9]+d[0-9]+)|(?:\+(?:STR)|(?:DEX)|(?:CON)|(?:WIS)|(?:INT)|(?:CHA)|(?:MAS)))*$", RegexOptions.IgnoreCase);

    public string Expression { get; set; } = "";


}
