using UndoableMediator.Commands;

namespace DnDActions.HitPointActions.RegainHp;

public class RegainHpCommand : HitPointCommandBase
{
    /// <summary>
    ///     This is the expected amount of hp gained, could be lowered in reality if the amount of Hp is too high
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    ///     This is the actual amount of hp gained
    /// </summary>
    public int? CorrectedAmount { get; set; }

    public RegainHpCommand(Guid targetId, int amount) : base(targetId)
    {
        Amount = amount;
    }

}
