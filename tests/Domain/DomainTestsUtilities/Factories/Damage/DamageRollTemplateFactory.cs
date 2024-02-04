using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DomainTestsUtilities.Factories.Damage;

public static class DamageRollTemplateFactory
{
    public static DamageRollTemplate Build(string? dicesToRoll = null, DamageTypeEnum? damageType = null)
    {
        return new DamageRollTemplate()
        {
            Dices = new DiceThrowTemplate(dicesToRoll ?? "2d8"),
            Type = damageType ?? DamageTypeEnum.Thunder,
        };
    }

    public static DamageRollTemplateCollection BuildCollection(DamageRollTemplate[] ? damageRollTemplates = null)
    {
        var damageRollTemplateCollection = new DamageRollTemplateCollection();
        if (damageRollTemplates != null)
        {
            damageRollTemplateCollection.AddRange(damageRollTemplates);
        }
        else
        {
            damageRollTemplateCollection.Add(Build());
        }

        return damageRollTemplateCollection;
    }
}
