using DnDEntities.Damage;

namespace Fight.MartialAttacks;

public class MartialAttackTemplate
{
	public MartialAttackTemplate()
	{
	}

    public DamageRollTemplateCollection Damages { get; set; } = new DamageRollTemplateCollection();

    public Guid Id { get; set; } = Guid.NewGuid();
}
