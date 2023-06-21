using Fight.Damage;
using Fight.Savings;

namespace DnDActions.DamageActions.ApplyDamageRollResults;

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
