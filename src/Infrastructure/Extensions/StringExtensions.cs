namespace Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Returns the given string, empty one when condition is false
    /// </summary>
    /// <param name="str"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static string When(this string str, bool condition)
    {
        return condition ? str : string.Empty;
    }
}
