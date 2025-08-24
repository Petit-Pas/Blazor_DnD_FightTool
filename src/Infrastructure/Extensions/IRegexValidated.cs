using System.Text.RegularExpressions;

namespace Extensions;

public interface IRegexValidated
{
    Regex Regex { get; }
    string Expression { get; set; }
}

public static class IRegexValidatedExtensions
{
    public static bool IsValid(this IRegexValidated validated, string input)
    {
        if (validated.Regex == null)
        {
            throw new InvalidOperationException("Regex is not set.");
        }
        return validated.Regex.IsMatch(input);
    }
}
