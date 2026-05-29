
namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainTempHp;

public class RegainTempHpAtomicCommand : TargetCommandBase<int>
{ 
    /// <summary>
    ///     The amount of Temporay hit points to provide
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    ///     The actual amount of temporary hit points that were provided
    /// </summary>
    public int? CorrectedAmount { get; set; }

    public RegainTempHpAtomicCommand(Guid targetId, int amount) : base(targetId)
    {
        Amount = amount;
    }
}
