using System.Text;

namespace DnDFightTool.UiTests.UiTestNavigation.Extensions;

/// <summary>
///     File-system helpers for locating folders and building safe path segments, so fixtures can express intent without
///     carrying directory-walking or character-sanitization mechanics inline.
/// </summary>
public static class FileExtensions
{
    /// <summary>
    ///     Walks up from <paramref name="start"/> (inclusive) to the nearest ancestor directory that contains at least one
    ///     file matching <paramref name="searchPattern"/>.
    /// </summary>
    /// <param name="start">The directory to start searching from.</param>
    /// <param name="searchPattern">The file search pattern to look for (e.g. <c>*.csproj</c>).</param>
    /// <returns>The nearest ancestor directory (including <paramref name="start"/>) that contains a matching file.</returns>
    /// <exception cref="InvalidOperationException">No ancestor contains a file matching the pattern.</exception>
    public static DirectoryInfo FindAncestorContaining(this DirectoryInfo start, string searchPattern)
    {
        var directory = start;
        while (directory is not null && !directory.EnumerateFiles(searchPattern).Any())
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException(
                $"No ancestor of '{start.FullName}' contains a file matching '{searchPattern}'.");
        }

        return directory;
    }

    /// <summary>
    ///     Replaces every character that is invalid in a file name with an underscore, yielding a value safe to use as a
    ///     single path segment.
    /// </summary>
    /// <param name="value">The raw name.</param>
    /// <returns>The sanitized name.</returns>
    public static string ToFileSafeName(this string value)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            builder.Append(Array.IndexOf(invalidCharacters, character) >= 0 ? '_' : character);
        }

        return builder.ToString();
    }
}
