using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks;

public class MartialAttackTemplate
{
	public MartialAttackTemplate()
	{
	}
    
    public ModifiersTemplate Modifiers { get; set; } = new ModifiersTemplate();

    public DamageRollTemplateCollection Damages { get; set; } = new DamageRollTemplateCollection();

    public Guid Id { get; set; } = Guid.NewGuid();
}
