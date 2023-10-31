using System.Diagnostics;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DnDFightTool.Domain.DnDEntities.Damage;

[DebuggerDisplay("{Dices}-{Type}")]
public class DamageRollTemplate
{
    public DiceThrowTemplate Dices { get; set; } = new DiceThrowTemplate("1d6+STR");
    public DamageTypeEnum Type { get; set; }

    public DamageRollResult GetEmptyRollResult()
    {
        return new DamageRollResult()
        {
            DamageType = Type,
            Dices = new DiceThrowTemplate(Dices.Expression),
        };
    }
}
