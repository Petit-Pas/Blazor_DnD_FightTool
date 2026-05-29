namespace DnDFightTool.Domain.Logs;

/// <summary>
///     A single log entry with tokenized content and indentation level.
/// </summary>
/// <param name="Id">Unique identifier, system-generated at creation time.</param>
/// <param name="Content">Tokenized text with BBCode-like formatting tags.</param>
/// <param name="IndentLevel">Indentation depth (0 = root, incremented by scopes).</param>
public record LogEntry(Guid Id, string Content, int IndentLevel);
