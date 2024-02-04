namespace DnDFightTool.Domain.DnDEntities.Dices;

/// <summary>
///     Represents a amount of a given dice. 
///     Example: 2d6 OR 1d4 OR 3d8
/// </summary>
public class Dices
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="amount"> The amount of dices of this size </param>
    /// <param name="value"> The size of the dice (max = value, min = 1) </param>
    public Dices(int amount, int value)
    {
        Amount = amount;
        Value = value;
    }

    /// <summary>
    ///     The amount of dices of this size
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    ///     The size of the dice (max = value, min = 1)
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    ///     Can make sure that a positive amount uses the '+' 
    /// </summary>
    /// <param name="enforceSign"> When set to true, 2d8 will be written +2d8 </param>
    /// <returns></returns>
    public string ToString(bool enforceSign)
    {
        return $"{(enforceSign && Amount >= 0 ? "+" : "")}{Amount}d{Value}";
    }

    /// <summary>
    ///    Override of ToString to use our overload no matter what
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return ToString(false);
    }

    /// <summary>
    ///     The maximum possible roll with these dices
    /// </summary>
    /// <returns></returns>
    public int MaximumRoll()
    {
        return Amount * Value;
    }
}
