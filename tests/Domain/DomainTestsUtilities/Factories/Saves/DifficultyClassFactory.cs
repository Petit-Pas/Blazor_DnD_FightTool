using DnDFightTool.Domain.CharacterSheet.Saves;

namespace DomainTestsUtilities.Factories.Saves;

public static class DifficultyClassFactory
{
    public static DifficultyClassTemplate Build(string? difficulty = null)
    {
        return new DifficultyClassTemplate(difficulty ?? "8+MAS+WIS");
    }
}
