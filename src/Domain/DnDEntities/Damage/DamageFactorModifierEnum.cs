namespace DnDEntities.Damage;

public enum DamageFactorModifierEnum
{
    [DamageFactorModifier(2)] Weak,
    [DamageFactorModifier(1)] Normal,
    [DamageFactorModifier(0.5)] Resistant,
    [DamageFactorModifier(0)] Immune
}