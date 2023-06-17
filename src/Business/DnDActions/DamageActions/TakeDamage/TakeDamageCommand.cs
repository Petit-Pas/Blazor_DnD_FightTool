using UndoableMediator.Commands;

namespace DnDActions.DamageActions.TakeDamage;

public class TakeDamageCommand : TargetedCommandBase
{
	public TakeDamageCommand(Guid targetId, int damage) : base(targetId)
	{
        Damage = damage;
    }

    public int Damage { get; }
}
