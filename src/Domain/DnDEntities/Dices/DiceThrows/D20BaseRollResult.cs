namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

/// <summary>
///     Base class for all D20 rolls
/// </summary>
public abstract class D20BaseRollResult
{
    /// <summary>
    ///     Result of the dice roll
    /// </summary>
    public int Result { get; set; } = 0;
}
