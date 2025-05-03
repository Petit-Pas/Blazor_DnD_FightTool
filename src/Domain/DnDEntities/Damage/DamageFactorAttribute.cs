namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///     Attribute used to mark a property as a damage factor
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DamageFactorAttribute : Attribute
{
    /// <summary>
    ///     The value of the modifier applied by this attribute
    /// </summary>
    private readonly double _modifier;

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="modifier"></param>
    public DamageFactorAttribute(double modifier)
    {
        _modifier = modifier;
    }

    /// <summary>
    ///     A way to retrieve the modifier from the attribute
    /// </summary>
    /// <returns></returns>
    public DamageFactor GetModifier()
    {
        return new DamageFactor(_modifier);
    }
}