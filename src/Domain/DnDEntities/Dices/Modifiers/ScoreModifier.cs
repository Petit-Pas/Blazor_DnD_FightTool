namespace DnDFightTool.Domain.DnDEntities.Dices.Modifiers;

/// <summary>
///     Modifier meant to simply add an amount to a roll/score/whatever
/// </summary>
/// <param name="Modifier"> By how much it should increase the score </param>
public record ScoreModifier(int Modifier)
{
    /// <summary>
    ///    Empty modifier
    /// </summary>
    public readonly static ScoreModifier Empty = new(0);

    /// <summary>
    ///     Add + sign to the modifier when needed.
    /// </summary>
    public string ModifierString => (Modifier >= 0 ? "+" : "") + Modifier;

    /// <summary>
    ///     Applies the modifier to the provided value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ApplyTo(int value)
    {
        return value + Modifier;
    }

    /// <summary>
    ///     Implicit conversion to int to allow to use the modifier directly in a calculation
    /// </summary>
    /// <param name="modifier"></param>
    public static implicit operator int(ScoreModifier modifier)
    {
        return modifier.Modifier;
    }
}
