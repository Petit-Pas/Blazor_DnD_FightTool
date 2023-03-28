// ReSharper disable InconsistentNaming
namespace Fight.Damage;

public enum DamageTypeEnum
{
    Bludgeoning,
    Piercing,
    Slashing,
    Bludgeoning_Silver,
    Piercing_Silver,
    Slashing_Silver,
    Bludgeoning_Magic,
    Piercing_Magic,
    Slashing_Magic,
    Acid,
    Cold,
    Fire,
    Force,
    Lightning,
    Necrotic,
    Poison,
    Psychic,
    Radiant,
    Thunder
}

public static class DamageTypeEnumExtensions
{
    public static string ToReadableString(this DamageTypeEnum skill)
    {
        return skill.ToString().Replace("_", " ");
    }
}