using System.Diagnostics;

namespace DnDEntities.Dices;

public class Dices
{
    public Dices(int amount, int value)
    {
        Amount = amount;
        Value = value;
    }

    public int Amount { get; set; }
    public int Value { get; set; }

    /// <summary>
    ///     Wil make sure that a positive amount uses the '+' sign
    /// </summary>
    /// <param name="enforceSign"></param>
    /// <returns></returns>
    public string ToString(bool enforceSign)
    {
        return $"{(enforceSign && Amount >= 0 ? "+" : "")}{Amount}d{Value}";
    }

    public override string ToString()
    {
        return ToString(false);
    }
}
