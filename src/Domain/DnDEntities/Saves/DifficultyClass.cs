using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DnDFightTool.Domain.DnDEntities.Saves;

public class DifficultyClass
{
    public DifficultyClass()
    {
    }

    public DifficultyClass(string expression)
    {
        Expression = new DiceThrowTemplate(expression);
    }

    public DiceThrowTemplate Expression { get; set; } = new DiceThrowTemplate("8+MAS+WIS");

    public int GetValue(Character caster)
    {
        return Expression.GetScoreModifier(caster).Modifier;
    }
}
