using Fight.Damage;

namespace Characters.DamageAffinities;

public class DamageAffinitiesCollection : List<DamageAffinity>
{
    public DamageAffinitiesCollection()
    {
        this.AddRange(new[]
        {
            new DamageAffinity(DamageTypeEnum.Bludgeoning, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Piercing, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Slashing, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Bludgeoning, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Bludgeoning_Silver, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Bludgeoning_Magic, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Acid, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Cold, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Fire, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Force, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Lightning, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Necrotic, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Poison, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Psychic, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Radiant, DamageFactorModifierEnum.Normal),
            new DamageAffinity(DamageTypeEnum.Thunder, DamageFactorModifierEnum.Normal),
        });
    }

    private DamageAffinity? GetDamageAffinityFor(DamageTypeEnum damageType)
    {
        var damageAffinity = this.SingleOrDefault(x => x.Type == damageType);
        if (damageAffinity == null)
        {
            // TODO warn
        }
        return damageAffinity;
    }

    public DamageFactorModifierEnum GetDamageFactorModifierFor(DamageTypeEnum damageType)
    {
        var damageAffinity = GetDamageAffinityFor(damageType);

        return damageAffinity?.Modifier ?? DamageFactorModifierEnum.Normal;
    }

    public DamageFactor GetDamageFactorFor(DamageTypeEnum damageType)
    {
        var damageAffinity = GetDamageAffinityFor(damageType);

        return damageAffinity?.GetDamageFactor() ?? DamageFactor.Empty;
    }
}