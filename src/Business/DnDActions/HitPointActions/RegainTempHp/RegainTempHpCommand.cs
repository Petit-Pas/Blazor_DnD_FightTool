namespace DnDFightTool.Business.DnDActions.HitPointActions.RegainTempHp;

/// <summary>
///     Orchestrator command: grants a fighter temporary hit points and logs the gain.
///     Dispatches <see cref="RegainTempHpAtomicCommand"/> (mutation) and a <see cref="DnDFightTool.Business.DnDActions.LogActions.WriteLog.WriteLogCommand"/> (log) as sub-commands.
/// </summary>
public class RegainTempHpCommand : TargetCommandBase
{
    /// <summary>The requested amount of temporary HP to grant.</summary>
    public int Amount { get; }

    public RegainTempHpCommand(Guid targetId, int amount) : base(targetId)
    {
        Amount = amount;
    }
}
