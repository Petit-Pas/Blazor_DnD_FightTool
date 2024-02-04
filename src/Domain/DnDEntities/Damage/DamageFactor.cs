namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///     
/// </summary>
public record class DamageFactor
{
    /// <summary>
    ///    Damage will remain as it is when this factor is applied
    /// </summary>
    public const double DoNothingFactor = 1;
    public static readonly DamageFactor DoNothing = new(DoNothingFactor);

    /// <summary>
    ///     Damage will be halved when this factor is applied
    /// </summary>
    public const double HalfFactor = 0.5;
    public static readonly DamageFactor Half = new(HalfFactor);

    /// <summary>
    ///     Damage will be doubled when this factor is applied
    /// </summary>
    public const double DoubleFactor = 2;
    public static readonly DamageFactor Double = new(DoubleFactor);

    /// <summary>
    ///     Damage will be nullified when this factor is applied
    /// </summary>
    public const double NullifyFactor = 0;
    public static readonly DamageFactor Nullify = new(NullifyFactor);

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="factor"> by how much damage should be multiplied </param>
    public DamageFactor(double factor)
    {
        _factor = factor;
    }

    /// <summary>
    ///     The stored factor
    /// </summary>
    private double _factor;

    /// <summary>
    ///     Allows to apply the factor to a damage value
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <returns></returns>
    public double ApplyOn(int baseDamage)
    {
        return baseDamage * _factor;
    }

    /// <summary>
    ///     Allows to apply the factor to a damage value
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <returns></returns>
    public double ApplyOn(double baseDamage)
    {
        return baseDamage * _factor;
    }

    /// <summary>
    ///     Implicit conversion to double using the Factor property
    /// </summary>
    /// <param name="d"></param>
    public static implicit operator double(DamageFactor d)
    {
        return d._factor;
    }
}