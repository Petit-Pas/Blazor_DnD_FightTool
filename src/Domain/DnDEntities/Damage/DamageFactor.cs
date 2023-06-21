namespace DnDEntities.Damage;

public class DamageFactor
{
    public DamageFactor(double factor)
    {
        Factor = factor;
    }

    private double Factor { get; init; }

    public static readonly DamageFactor None = new (1);

    public double ApplyOn(int baseDamage)
    {
        return baseDamage * Factor;
    }

    public double ApplyOn(double baseDamage)
    {
        return baseDamage * Factor;
    }
}