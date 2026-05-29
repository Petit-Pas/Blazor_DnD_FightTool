namespace DnDFightTool.Domain.Logs;

/// <summary>
///     In-memory implementation of <see cref="IDnDLogService"/>.
///     Singleton, session-scoped.
/// </summary>
public class DnDLogService : IDnDLogService
{
    private readonly List<LogBlock> _blocks = [];
    private readonly HashSet<Guid> _hiddenIds = [];
    private LogBlock? _currentBlock;
    private int _indentLevel;

    /// <inheritdoc />
    public IReadOnlyList<LogBlock> Blocks => _blocks;

    /// <inheritdoc />
    public event Action? OnChanged;

    /// <inheritdoc />
    public Guid OpenBlock(string name)
    {
        if (_currentBlock is not null)
        {
            throw new InvalidOperationException("Cannot open a new block while another block is already open. Call CloseBlock() first.");
        }

        _currentBlock = new LogBlock(name);
        _blocks.Add(_currentBlock);
        _indentLevel = 0;
        return _currentBlock.Id;
    }

    /// <inheritdoc />
    public Guid CloseBlock()
    {
        if (_currentBlock is null)
        {
            throw new InvalidOperationException("No block is currently open.");
        }

        var closedBlockId = _currentBlock.Id;
        _currentBlock = null;
        _indentLevel = 0;
        return closedBlockId;
    }

    /// <inheritdoc />
    public void ReopenBlock(Guid blockId)
    {
        var block = _blocks.FirstOrDefault(b => b.Id == blockId) ?? throw new InvalidOperationException($"Block with ID {blockId} not found.");
        
        _currentBlock = block;
        _indentLevel = 0;
    }

    /// <inheritdoc />
    public void OpenScope()
    {
        _indentLevel++;
    }

    /// <inheritdoc />
    public void CloseScope()
    {
        if (_indentLevel <= 0)
        {
            throw new InvalidOperationException("No scope is currently open.");
        }

        _indentLevel--;
    }

    /// <inheritdoc />
    public Guid AddEntry(string content)
    {
        if (_currentBlock is null)
        {
            // No block open — create a transient anonymous block for top-level entries
            // (e.g., round headers that appear between turn blocks).
            // Does NOT set _currentBlock so the next OpenBlock() call still succeeds.
            var anonymousBlock = new LogBlock(string.Empty);
            _blocks.Add(anonymousBlock);
            var freeEntry = new LogEntry(Guid.NewGuid(), content, 0);
            anonymousBlock.AddEntry(freeEntry);
            OnChanged?.Invoke();
            return freeEntry.Id;
        }

        var entry = new LogEntry(Guid.NewGuid(), content, _indentLevel);
        _currentBlock.AddEntry(entry);
        OnChanged?.Invoke();
        return entry.Id;
    }

    /// <inheritdoc />
    public void Hide(Guid id)
    {
        if (_hiddenIds.Add(id))
        {
            OnChanged?.Invoke();
        }
    }

    /// <inheritdoc />
    public void Show(Guid id)
    {
        if (_hiddenIds.Remove(id))
        {
            OnChanged?.Invoke();
        }
    }

    /// <inheritdoc />
    public bool IsHidden(Guid id)
    {
        return _hiddenIds.Contains(id);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _blocks.Clear();
        _hiddenIds.Clear();
        _currentBlock = null;
        _indentLevel = 0;
        OnChanged?.Invoke();
    }
}
