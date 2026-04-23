namespace DnDFightTool.Domain.Rolls;

/// <summary>
///     Base class for all D20 rolls
/// </summary>
public abstract class D20BaseRollResult : IDiceRollResult
{
    /// <inheritdoc />
    public int Result { get; set; } = 0;

    /// <inheritdoc />
    public int Min => 1;

    /// <inheritdoc />
    public int Max => 20;
}
