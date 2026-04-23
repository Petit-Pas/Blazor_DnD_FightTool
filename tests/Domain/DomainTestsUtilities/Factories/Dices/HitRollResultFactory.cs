using DnDFightTool.Domain.Rolls;
using DnDFightTool.Domain.CharacterSheet.Dices;

namespace DomainTestsUtilities.Factories.Dices;

public static class HitRollResultFactory
{
    public static HitRollResult Build(int? result = null, DiceRollModifiersTemplate? modifiers = null)
    {
        return new HitRollResult()
        {
            Result = result ?? 10,
            Modifiers = modifiers ?? ModifiersTemplateFactory.Build()
        };
    }
}
