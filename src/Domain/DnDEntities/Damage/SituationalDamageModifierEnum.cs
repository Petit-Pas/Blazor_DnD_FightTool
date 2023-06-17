namespace DnDEntities.Damage;

public enum SituationalDamageModifierEnum
{
    [DamageFactor(1)]   Normal,
    [DamageFactor(0.5)] Halved,
    [DamageFactor(0)]   Canceled
}
