namespace DnDFightTool.Domain.Logs;

/// <summary>
///     Service for managing structured game-event log entries.
///     Singleton, session-scoped, in-memory.
/// </summary>
public interface IDnDLogService
{
    /// <summary>
    ///     All blocks in creation order.
    /// </summary>
    IReadOnlyList<LogBlock> Blocks { get; }

    /// <summary>
    ///     Fired after any mutation (add entry, hide, show, clear).
    /// </summary>
    event Action? OnChanged;

    /// <summary>
    ///     Start a new named block; subsequent entries belong to it.
    /// </summary>
    /// <param name="name">Display name for the block.</param>
    /// <returns>The new block's unique identifier.</returns>
    Guid OpenBlock(string name);

    /// <summary>
    ///     Close the current block.
    /// </summary>
    /// <returns>The closed block's <see cref="LogBlock.Id"/>.</returns>
    Guid CloseBlock();

    /// <summary>
    ///     Re-open a previously closed block by setting it as the current block.
    /// </summary>
    /// <param name="blockId">The block's unique identifier.</param>
    /// <exception cref="InvalidOperationException">Thrown if the block is not found.</exception>
    void ReopenBlock(Guid blockId);

    /// <summary>
    ///     Increase indent level for subsequent entries.
    /// </summary>
    void OpenScope();

    /// <summary>
    ///     Decrease indent level.
    /// </summary>
    void CloseScope();

    /// <summary>
    ///     Create a new <see cref="LogEntry"/> in the current block at the current indent level.
    ///     If no block is currently open, the entry is written into a transient anonymous block
    ///     (indent level 0) so that top-level entries such as round headers can appear between turn blocks.
    ///     The anonymous block does <b>not</b> become the current block; the next <see cref="OpenBlock"/> call still succeeds.
    /// </summary>
    /// <param name="content">Tokenized text with BBCode-like formatting tags.</param>
    /// <returns>The entry's unique identifier.</returns>
    Guid AddEntry(string content);

    /// <summary>
    ///     Mark an entry as hidden. Idempotent.
    /// </summary>
    /// <param name="id">The entry identifier.</param>
    void Hide(Guid id);

    /// <summary>
    ///     Mark an entry as visible. Idempotent.
    /// </summary>
    /// <param name="id">The entry identifier.</param>
    void Show(Guid id);

    /// <summary>
    ///     Check if an entry is currently hidden.
    /// </summary>
    /// <param name="id">The entry identifier.</param>
    /// <returns><c>true</c> if the entry is hidden; otherwise <c>false</c>.</returns>
    bool IsHidden(Guid id);

    /// <summary>
    ///     Remove all entries, blocks, and hidden state permanently.
    /// </summary>
    void Clear();
}
