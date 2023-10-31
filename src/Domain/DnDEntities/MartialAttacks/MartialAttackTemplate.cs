using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Statuses;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks;

public class MartialAttackTemplate : IHashable
{
	public MartialAttackTemplate()
	{
	}

    public string Name { get; set; }

    public ModifiersTemplate Modifiers { get; set; } = new ModifiersTemplate();

    public DamageRollTemplateCollection Damages { get; set; } = new DamageRollTemplateCollection();

    public StatusTemplateCollection Statuses { get; set; } = new StatusTemplateCollection();

    public Guid Id { get; set; } = Guid.NewGuid();
}
