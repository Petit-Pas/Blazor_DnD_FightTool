namespace DnDFightTool.Domain.DnDEntities.Dices;

/// <summary>
///     D20 roll result with modifiers applied on top.
///     Used for attack rolls and other rolls that have a modifier expression.
/// </summary>
public class ModifiedD20RollResult : D20BaseRollResult
{
    /// <summary>
    ///     Empty ctor
    /// </summary>
    public ModifiedD20RollResult()
    {
    }

    /// <summary>
    ///     Ctor that allows for modifiers to be passed
    /// </summary>
    /// <param name="modifiersTemplate"> modifiers Example: STR+MAS+1 </param>
    public ModifiedD20RollResult(DiceRollModifiersTemplate modifiersTemplate)
    {
        Modifiers = modifiersTemplate;
    }

    /// <summary>
    ///     Modifiers to be applied to the roll
    /// </summary>
    public DiceRollModifiersTemplate Modifiers { get; set; } = new DiceRollModifiersTemplate();
}
