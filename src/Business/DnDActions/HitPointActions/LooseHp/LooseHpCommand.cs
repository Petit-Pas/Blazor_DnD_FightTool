using UndoableMediator.Commands;

namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;

public class LooseHpCommand : HitPointCommandBase
{
    /// <summary>
    ///     This is the expected amount of hp removed, could be lowered in reality if the amount of Hp is not sufficient
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    ///     This is the actual amount of HP that were removed
    /// </summary>
    public int? CorrectedAmount { get; set; }

    /// <summary>
    ///     The guid of the actual target
    /// </summary>
    public LooseHpCommand(Guid targetId, int amount) : base(targetId)
    { 
        Amount = amount;
    }
}
