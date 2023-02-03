namespace Fight.Damage;

public class DamageFactorModifierAttribute : Attribute
{
    private readonly double _modifier;

    public double GetModifier() => _modifier;

    public DamageFactorModifierAttribute(double modifier)
    {
        _modifier = modifier;
    }
}