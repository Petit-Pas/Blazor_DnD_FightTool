using DnDFightTool.Domain.DnDEntities.AbilityScores;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Saves;

namespace DomainTestsUtilities.Fakes.Savings;

public class FakeSaveRollResult : SaveRollResult
{
    private readonly bool _isSuccessful;

    public FakeSaveRollResult(bool isSuccessful) : base(new DifficultyClass("10"), AbilityEnum.Strength)
    {
        _isSuccessful = isSuccessful;
    }

    public override bool IsSuccessful(Character targetCharacter, Character casterCharacter)
    {
        return _isSuccessful;
    }
}
