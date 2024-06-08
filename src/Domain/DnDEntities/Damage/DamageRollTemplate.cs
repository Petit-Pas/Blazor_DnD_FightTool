using System.Diagnostics;
using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Damage;

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
    public DiceThrowTemplate Dices { get; set; } = new DiceThrowTemplate("1d6+STR");

    /// <summary>
    ///     The type of the damage for this specific damage roll.
    /// </summary>
    public DamageTypeEnum Type { get; set; }

    /// <summary>
    ///     Creates an empty <see cref="DamageRollResult"/> with the same dices and type as this template.
    /// </summary>
    /// <returns></returns>
    public DamageRollResult GetEmptyRollResult()
    {
        return new DamageRollResult(Dices, Type);
    }
}
