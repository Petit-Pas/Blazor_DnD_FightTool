namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainHp;

public class RegainHpAtomicCommand : TargetCommandBase<int>
{
    /// <summary>
    ///     This is the expected amount of hp gained, could be lowered in reality if the amount of Hp is too high
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    ///     This is the actual amount of hp gained
    /// </summary>
    public int? CorrectedAmount { get; set; }

    public RegainHpAtomicCommand(Guid targetId, int amount) : base(targetId)
    {
        Amount = amount;
    }

}
