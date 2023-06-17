namespace DnDEntities.Damage;

public class DamageFactorAttribute : Attribute
{
    private readonly double _modifier;

    public double GetModifier() => _modifier;

    public DamageFactorAttribute(double modifier)
    {
        _modifier = modifier;
    }
}