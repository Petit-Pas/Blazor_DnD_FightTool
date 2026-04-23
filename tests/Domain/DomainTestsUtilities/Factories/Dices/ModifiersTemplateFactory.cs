using DnDFightTool.Domain.CharacterSheet.Dices;

namespace DomainTestsUtilities.Factories.Dices;

public static class ModifiersTemplateFactory
{
    public static DiceRollModifiersTemplate Build(string? expression = null)
    {
        return new DiceRollModifiersTemplate(expression ?? "8+WIS");
    }
}
