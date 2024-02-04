using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DomainTestsUtilities.Factories.Dices.DiceThrows;

public static class HitRollResultFactory
{
    public static HitRollResult Build(int? result = null, ModifiersTemplate? modifiers = null)
    {
        return new HitRollResult()
        {
            Result = result ?? 10,
            Modifiers = modifiers ?? ModifiersTemplateFactory.Build()
        };
    }
}
