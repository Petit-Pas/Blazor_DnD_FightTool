using DnDEntities.Dices.DiceThrows;
using Fight.Damage;

namespace DnDEntities.Damage;

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
