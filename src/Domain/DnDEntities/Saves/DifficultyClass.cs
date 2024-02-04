using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Saves;

/// <summary>
///     Difficulty class
///     Meant to be used as a target for a <see cref="SaveRollResult"/>
///     Is described by a <see cref="ModifiersTemplate"/> so it can be numbers & wildcards
/// </summary>
public class DifficultyClass : IHashable
{
    /// <summary>
    ///     Empty Ctor
    /// </summary>
    public DifficultyClass()
    {
    }

    /// <summary>
    ///     Ctor that allows for an expression
    /// </summary>
    /// <param name="expression"></param>
    public DifficultyClass(string expression)
    {
        DifficultyClassExpression = new ModifiersTemplate(expression);
    }

    /// <summary>
    ///     The expression describing the difficulty class
    /// </summary>
    public ModifiersTemplate DifficultyClassExpression { get; set; } = new ModifiersTemplate("DC");

    /// <summary>
    ///    Get the actual value of the difficulty class by evaluating the wildcards in the expression
    /// </summary>
    /// <param name="caster"> Should be the character that prompts for a saving, since it's his wildcards that need resolving. </param>
    /// <returns></returns>
    public int GetValue(Character caster)
    {
        return DifficultyClassExpression.GetScoreModifier(caster).Modifier;
    }
}
