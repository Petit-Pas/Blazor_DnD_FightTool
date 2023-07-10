using DnDEntities.Damage;
using DnDEntities.Dices.DiceThrows;

namespace Fight.MartialAttacks;

public class MartialAttackTemplate
{
	public MartialAttackTemplate()
	{
	}
    
    public ModifiersTemplate Modifiers { get; set; } = new ModifiersTemplate();

    public DamageRollTemplateCollection Damages { get; set; } = new DamageRollTemplateCollection();

    public Guid Id { get; set; } = Guid.NewGuid();
}
