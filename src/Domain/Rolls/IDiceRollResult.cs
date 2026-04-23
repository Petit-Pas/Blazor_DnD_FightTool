namespace DnDFightTool.Domain.Rolls;

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

    /// <summary>
    ///     <c>true</c> when <see cref="Result"/> is within the valid [<see cref="Min"/>, <see cref="Max"/>] range.
    /// </summary>
    bool IsRolled => Result >= Min && Result <= Max;

    /// <summary>
    ///     Sets <see cref="Result"/> to a random value in [<see cref="Min"/>, <see cref="Max"/>].
    ///     Does nothing if the result is already valid (i.e. <see cref="IsRolled"/> is <c>true</c>).
    /// </summary>
    void Roll()
    {
        if (!IsRolled)
        {
            Result = Random.Shared.Next(Min, Max + 1);
        }
    }
}
