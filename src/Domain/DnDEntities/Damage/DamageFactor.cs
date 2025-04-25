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
    public readonly static DamageFactor DoNothing = new(DoNothingFactor);

    /// <summary>
    ///     Damage will be halved when this factor is applied
    /// </summary>
    public const double HalfFactor = 0.5;
    public readonly static DamageFactor Half = new(HalfFactor);

    /// <summary>
    ///     Damage will be doubled when this factor is applied
    /// </summary>
    public const double DoubleFactor = 2;
    public readonly static DamageFactor Double = new(DoubleFactor);

    /// <summary>
    ///     Damage will be nullified when this factor is applied
    /// </summary>
    public const double NullifyFactor = 0;
    public readonly static DamageFactor Nullify = new(NullifyFactor);

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="factor"> by how much damage should be multiplied </param>
    public DamageFactor(double factor)
    {
        Factor = factor;
    }

    /// <summary>
    ///     The stored factor
    /// </summary>
    public double Factor { get ; private set; }

    /// <summary>
    ///     Allows to apply the factor to a damage value
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <returns></returns>
    public double ApplyOn(int baseDamage)
    {
        return baseDamage * Factor;
    }

    /// <summary>
    ///     Allows to apply the factor to a damage value
    /// </summary>
    /// <param name="baseDamage"></param>
    /// <returns></returns>
    public double ApplyOn(double baseDamage)
    {
        return baseDamage * Factor;
    }

    /// <summary>
    ///     Implicit conversion to double using the Factor property
    /// </summary>
    /// <param name="d"></param>
    public static implicit operator double(DamageFactor d)
    {
        return d.Factor;
    }
}