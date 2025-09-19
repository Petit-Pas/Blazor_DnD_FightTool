using DnDFightTool.Domain.DnDEntities.Damage;

namespace DnDFightTool.Domain.Fight.DomainExtensions.Damage;

public static class DamageRollTemplateExtensions
{
    public static DamageRollResult[] GetRollableResult(this DamageRollTemplateCollection damageRollTemplateCollection)
    {
        return [.. damageRollTemplateCollection.Select(x => x.GetEmptyRollResult())];
    }
}
