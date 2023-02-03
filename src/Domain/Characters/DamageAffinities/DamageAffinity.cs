using Extensions;
using Fight.Damage;

namespace Characters.DamageAffinities;

public class DamageAffinity
{
    public DamageAffinity(DamageTypeEnum type, DamageFactorModifierEnum modifier)
    {
        Type = type;
        Modifier = modifier;
    }

    public DamageFactor GetDamageFactor()
    {
        var modifier = Modifier.GetAttribute<DamageFactorModifierAttribute>();
        if (modifier == null)
        {
            Console.WriteLine($"WARNING: the skill {Type} has an unknown damage factor modifier.");
            return DamageFactor.Empty;
        }

        return new DamageFactor(modifier.GetModifier());
    }

    public DamageTypeEnum Type { get; set; }
    public DamageFactorModifierEnum Modifier { get; set; }
}