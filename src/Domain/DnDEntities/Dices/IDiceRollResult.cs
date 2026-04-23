namespace DnDFightTool.Domain.DnDEntities.Dices;

/// <summary>
///     Represents the result of a dice roll with its valid range.
/// </summary>
public interface IDiceRollResult
{
    /// <summary>
    ///     The rolled value.
    /// </summary>
    int Result { get; set; }

    /// <summary>
    ///     Minimum valid value for this roll.
    /// </summary>
    int Min { get; }

    /// <summary>
    ///     Maximum valid value for this roll.
    /// </summary>
    int Max { get; }
}
