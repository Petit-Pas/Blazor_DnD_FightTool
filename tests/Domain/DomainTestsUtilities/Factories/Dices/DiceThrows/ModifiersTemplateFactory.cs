using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DomainTestsUtilities.Factories.Dices.DiceThrows;

public static class ModifiersTemplateFactory
{
    static public ModifiersTemplate Build(string? expression = null)
    {
        return new ModifiersTemplate(expression ?? "8+WIS");
    }
}
