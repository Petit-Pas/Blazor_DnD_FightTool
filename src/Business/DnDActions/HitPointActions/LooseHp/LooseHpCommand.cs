namespace DnDFightTool.Business.DnDActions.HitPointActions.LooseHp;

/// <summary>
///     Orchestrator command: reduces a fighter's hit points and logs the loss.
///     Dispatches <see cref="LooseHpAtomicCommand"/> (mutation) and a <see cref="DnDFightTool.Business.DnDActions.LogActions.WriteLog.WriteLogCommand"/> (log) as sub-commands.
/// </summary>
public class LooseHpCommand : TargetCommandBase
{
    /// <summary>The requested amount of HP to remove.</summary>
    public int Amount { get; }

    public LooseHpCommand(Guid targetId, int amount) : base(targetId)
    {
        Amount = amount;
    }
}
