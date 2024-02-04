using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Saves;

namespace DomainTestsUtilities.Factories.Saves;

public static class SaveRollTemplateFactory
{
    public static SaveRollTemplate Build(
        DifficultyClass? difficulty = null,
        AbilityEnum? targetAbility = null
        )
    {
        return new SaveRollTemplate()
        {
            Difficulty = difficulty ?? DifficultyClassFactory.Build(),
            TargetAbility = targetAbility ?? AbilityEnum.Dexterity
        };
    }
}
