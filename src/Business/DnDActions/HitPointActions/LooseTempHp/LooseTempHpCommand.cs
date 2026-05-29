namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseTempHp;

/// <summary>
///     Orchestrator command: reduces a fighter's temporary hit points and logs the loss.
///     Dispatches <see cref="LooseTempHpAtomicCommand"/> (mutation) and a <see cref="DnDFightTool.Business.DnDActions.LogActions.WriteLog.WriteLogCommand"/> (log) as sub-commands.
/// </summary>
public class LooseTempHpCommand : TargetCommandBase
{
    /// <summary>The requested amount of temp HP to remove.</summary>
    public int Amount { get; }

    public LooseTempHpCommand(Guid targetId, int amount) : base(targetId)
    {
        Amount = amount;
    }
}
