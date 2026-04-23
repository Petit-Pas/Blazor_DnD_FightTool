using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Saves;
using DnDFightTool.Domain.Rolls;

namespace DomainTestsUtilities.Factories.Saves;

public static class SaveRollResultFactory
{
    public static SaveRollResult Build(
        DifficultyClassTemplate? difficultyClass = null,
        AbilityEnum? ability = null,
        int? rolledResult = null
        )
    {
        return new SaveRollResult(difficultyClass ?? DifficultyClassFactory.Build(), ability ?? AbilityEnum.Wisdom)
        {
            Result = rolledResult ?? 0
        };
    }
}
