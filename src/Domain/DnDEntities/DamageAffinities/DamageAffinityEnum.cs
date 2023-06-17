using DnDEntities.Damage;

namespace DnDEntities.DamageAffinities;

public enum DamageAffinityEnum
{
    [DamageFactor(2)]   Weak,
    [DamageFactor(1)]   Normal,
    [DamageFactor(0.5)] Resistant,
    [DamageFactor(0)]   Immune
}