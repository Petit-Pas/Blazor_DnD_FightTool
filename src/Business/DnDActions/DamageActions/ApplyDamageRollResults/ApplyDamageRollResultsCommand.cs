using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Saves;

namespace DnDFightTool.Business.DnDActions.DamageActions.ApplyDamageRollResults;

public class ApplyDamageRollResultsCommand : TargetCasterCommandBase
{
	public ApplyDamageRollResultsCommand(Guid targetId, Guid casterId, DamageRollResult[] damageRolls, SaveRollResult? saving = null) : base(targetId, casterId)
	{
        DamageRolls = damageRolls;
        Save = saving;
    }

    public DamageRollResult[] DamageRolls { get; }

    public SaveRollResult? Save { get; }
}
