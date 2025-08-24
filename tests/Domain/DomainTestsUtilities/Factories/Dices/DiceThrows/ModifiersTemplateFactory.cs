using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DomainTestsUtilities.Factories.Dices.DiceThrows;

public static class ModifiersTemplateFactory
{
    public static DiceThrowModifiersTemplate Build(string? expression = null)
    {
        return new DiceThrowModifiersTemplate(expression ?? "8+WIS");
    }
}
