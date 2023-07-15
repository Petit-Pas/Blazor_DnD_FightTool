namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

public abstract class D20BaseRollResult
{
    public D20BaseRollResult()
    {
    }

    public D20BaseRollResult(int result)
    {
        Result = result;
    }

    public int Result { get; set; } = 0;
}
