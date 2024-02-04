using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Saves;

namespace DomainTestsUtilities.Factories.Saves;

public static class SaveRollResultFactory
{
    public static SaveRollResult Build(
        DifficultyClass? difficultyClass = null,
        AbilityEnum? ability = null,
        int? rolledResult = null
        )
    {
        return new SaveRollResult(difficultyClass ?? DifficultyClassFactory.Build(), ability ?? AbilityEnum.Wisdom)
        {
            RolledResult = rolledResult ?? 0
        };
    }
}
