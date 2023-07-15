using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DnDFightTool.Domain.DnDEntities.Damage;

public class DamageRollTemplate
{
    public DiceThrowTemplate Dices { get; set; }
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
