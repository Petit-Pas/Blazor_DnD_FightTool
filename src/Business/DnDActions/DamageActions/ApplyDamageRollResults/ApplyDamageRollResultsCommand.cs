using DnDEntities.Damage;
using Fight.Damage;

namespace DnDActions.DamageActions.ApplyDamageRollResults;

public class ApplyDamageRollResultsCommand : TargetedCommandBase
{
	public ApplyDamageRollResultsCommand(Guid targetId, DamageRollResult[] damageRolls) : base(targetId)
	{
        DamageRolls = damageRolls;
    }

    public DamageRollResult[] DamageRolls { get; }
}
