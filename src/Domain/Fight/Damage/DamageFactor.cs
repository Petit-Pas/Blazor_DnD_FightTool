namespace Fight.Damage;

public record DamageFactor (double Factor)
{
    public static readonly DamageFactor Empty = new (1);
}