using Extensions;

namespace DnDEntities.Damage;

public enum SituationalDamageModifierEnum
{
    [DamageFactor(1)]   Normal,
    [DamageFactor(0.5)] Halved,
    [DamageFactor(0)]   Canceled
}

public static class SituationalDamageModifierEnumExtensions
{
    public static DamageFactor GetFactor(this SituationalDamageModifierEnum damageModifier)
    {
        var factorAttribute = damageModifier.GetAttribute<DamageFactorAttribute>();
        if (factorAttribute == null)
        {
            Console.WriteLine($"WARNING: the SituationalDamageModifierEnum {damageModifier} has no DamageAffinitFactory attribute.");
            return DamageFactor.None;
        }
        return new DamageFactor(factorAttribute.GetModifier());
    }
}
