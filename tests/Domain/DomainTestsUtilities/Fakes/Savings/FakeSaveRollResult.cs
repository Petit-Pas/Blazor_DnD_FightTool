using DnDFightTool.Domain.CharacterSheet.AbilityScores;
using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Saves;
using DnDFightTool.Domain.Rolls;

namespace DomainTestsUtilities.Fakes.Savings;

public class FakeSaveRollResult : SaveRollResult
{
    private readonly bool _isSuccessful;

    public FakeSaveRollResult(bool isSuccessful) : base(new DifficultyClassTemplate("10"), AbilityEnum.Strength)
    {
        _isSuccessful = isSuccessful;
    }

    public override bool IsSuccessful(ICharacter targetCharacter, ICharacter casterCharacter)
    {
        return _isSuccessful;
    }
}
