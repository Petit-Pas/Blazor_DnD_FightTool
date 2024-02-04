using DnDFightTool.Domain.DnDEntities.Saves;

namespace DomainTestsUtilities.Factories.Saves;

public static class DifficultyClassFactory
{
    public static DifficultyClass Build(string? difficulty = null)
    {
        return new DifficultyClass(difficulty ?? "8+MAS+WIS");
    }
}
