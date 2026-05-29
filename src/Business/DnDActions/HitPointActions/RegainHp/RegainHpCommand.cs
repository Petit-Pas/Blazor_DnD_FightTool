namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainHp;

/// <summary>
///     Orchestrator command: restores a fighter's hit points and logs the gain.
///     Dispatches <see cref="RegainHpAtomicCommand"/> (mutation) and a <see cref="DnDFightTool.Business.DnDActions.LogActions.WriteLog.WriteLogCommand"/> (log) as sub-commands.
/// </summary>
public class RegainHpCommand : TargetCommandBase
{
    /// <summary>The requested amount of HP to restore.</summary>
    public int Amount { get; }

    public RegainHpCommand(Guid targetId, int amount) : base(targetId)
    {
        Amount = amount;
    }
}
