using DnDFightTool.Domain.Rolls;

namespace DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;

public class ApplyDamageRollResultsCommand : CasterTargetCommandBase
{
	public ApplyDamageRollResultsCommand(Guid casterId, Guid targetId, DamageRollResult[] damageRolls, SaveRollResult? saving = null) : base(casterId, targetId)
	{
        DamageRolls = damageRolls;
        Save = saving;
    }

    public DamageRollResult[] DamageRolls { get; }

    public SaveRollResult? Save { get; }
}
