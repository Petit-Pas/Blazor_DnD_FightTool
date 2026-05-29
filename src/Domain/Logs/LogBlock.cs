namespace DnDFightTool.Domain.Logs;

/// <summary>
///     A named group of related log entries.
/// </summary>
public class LogBlock
{
    private readonly List<LogEntry> _entries = [];

    /// <summary>
    ///     Unique identifier, system-generated at creation time.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Display name for this block (e.g., "Martial Attack").
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Ordered list of entries in this block.
    /// </summary>
    public IReadOnlyList<LogEntry> Entries => _entries;

    /// <summary>
    ///     Creates a new log block with the specified name.
    /// </summary>
    /// <param name="name">Display name for this block.</param>
    public LogBlock(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Adds an entry to this block.
    /// </summary>
    internal void AddEntry(LogEntry entry)
    {
        _entries.Add(entry);
    }
}
