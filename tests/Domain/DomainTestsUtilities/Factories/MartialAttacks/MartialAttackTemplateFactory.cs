using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices;
using DnDFightTool.Domain.DnDEntities.MartialAttacks;
using DnDFightTool.Domain.DnDEntities.Statuses;
using DomainTestsUtilities.Factories.Damage;
using DomainTestsUtilities.Factories.Dices;
using DomainTestsUtilities.Factories.Status;

namespace DomainTestsUtilities.Factories.MartialAttacks;

public static class MartialAttackTemplateFactory
{
    public static MartialAttackTemplate Build(
        string? name = null,
        DiceRollModifiersTemplate? modifiers = null,
        DamageRollTemplateCollection? damages = null,
        StatusTemplateCollection? statuses = null,
        Guid? id = null
        )
    {
        return new MartialAttackTemplate()
        {
            Name = name ?? "Attack template",
            ToHitModifiers = modifiers ?? ModifiersTemplateFactory.Build(),
            Damages = damages ?? DamageRollTemplateFactory.BuildCollection(),
            Statuses = statuses ?? StatusTemplateFactory.BuildCollection(),
            Id = id ?? Guid.NewGuid(),
        };
    }
}
