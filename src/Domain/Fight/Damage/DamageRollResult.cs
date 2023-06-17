
using DnDEntities.Damage;

namespace Fight.Damage;

public class DamageRollResult
{
    // Model is simplified for now, this represents the damage rolled
    public int Damage { get; set; }

    public DamageTypeEnum DamageType { get; set; }
}
