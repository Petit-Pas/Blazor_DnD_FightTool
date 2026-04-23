using DnDFightTool.Domain.CharacterSheet.Damage;
using DnDFightTool.Domain.CharacterSheet.Dices;
using DnDFightTool.Domain.CharacterSheet.Statuses;
using DnDFightTool.Infrastructure.Memory.Hashes;

namespace DnDFightTool.Domain.CharacterSheet.MartialAttacks;

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
    public DiceRollModifiersTemplate ToHitModifiers { get; set; } = new DiceRollModifiersTemplate();

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
