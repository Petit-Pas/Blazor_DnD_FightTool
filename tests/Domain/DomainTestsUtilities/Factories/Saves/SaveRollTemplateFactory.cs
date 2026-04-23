using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Saves;

namespace DomainTestsUtilities.Factories.Saves;

public static class SaveRollTemplateFactory
{
    public static SaveRollTemplate Build(
        DifficultyClassTemplate? difficulty = null,
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
