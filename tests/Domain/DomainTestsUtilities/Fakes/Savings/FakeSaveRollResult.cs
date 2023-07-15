using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Saves;

namespace DomainTestsUtilities.Fakes.Savings;

public class FakeSaveRollResult : SaveRollResult
{
    private readonly bool _isSuccesful;

    public FakeSaveRollResult(bool isSuccesful)
    {
        _isSuccesful = isSuccesful;
    }

    public override bool IsSuccesfull(Character targetCharacter, Character casterCharacter)
    {
        return _isSuccesful;
    }
}
