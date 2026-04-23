using System.Diagnostics;
using DnDFightTool.Domain.CharacterSheet.Dices;
using DnDFightTool.Infrastructure.Memory.Hashes;

namespace DnDFightTool.Domain.CharacterSheet.Damage;

/// <summary>
///     A template for a damage roll.
///     This only contains the information of the dices, and not the result of the roll.
///     <see cref="DamageRollResult"/> for the result of a roll."/>
/// </summary>
[DebuggerDisplay("{Dices}-{Type}")]
public class DamageRollTemplate : IHashable
{
    /// <summary>
    ///     The dices to throw
    /// </summary>
    public DiceRollTemplate Dices { get; set; } = new DiceRollTemplate("1d6+STR");

    /// <summary>
    ///     The type of the damage for this specific damage roll.
    /// </summary>
    public DamageTypeEnum Type { get; set; }
}
