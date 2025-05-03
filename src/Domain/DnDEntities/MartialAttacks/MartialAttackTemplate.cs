using DnDFightTool.Domain.DnDEntities.Damage;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using DnDFightTool.Domain.DnDEntities.Statuses;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.MartialAttacks;

/// <summary>
///     A template for a martial attack.
///     Should be made with a weapon or a bodily feature such as claws, bite, etc.
/// </summary>
public class MartialAttackTemplate : IHashable
{
    /// <summary>
    ///     Empty ctor
    /// </summary>
	public MartialAttackTemplate()
	{
	}

    /// <summary>
    ///     Meaningful name for the attack
    /// </summary>
    public string Name { get; set; } = "Attack template";

    /// <summary>
    ///     Modifiers to apply to the attack roll, supports wildcards and static modifiers
    /// </summary>
    public ModifiersTemplate Modifiers { get; set; } = new ModifiersTemplate();

    /// <summary>
    ///     The damage to apply to the target if the attack hits
    /// </summary>
    public DamageRollTemplateCollection Damages { get; set; } = [];

    /// <summary>
    ///     The statuses that the attack might apply to the target if the attack hits
    /// </summary>
    public StatusTemplateCollection Statuses { get; set; } = [];

    /// <summary>
    ///     A unique non meaningful identifier for this attack
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
}
