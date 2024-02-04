using DnDFightTool.Domain.DnDEntities.HitPoint.Validation;

namespace DnDFightTool.Domain.DnDEntities.Dices.DiceThrows.Validation;

/// <summary>
///     Validator for <see cref="HitRollResult" />
/// </summary>
public class HitRollResultValidator : D20RollResultValidator<HitRollResult>
{
    /// <summary>
    ///     Ctor that also calls base ctor since rules are defined there already
    /// </summary>
    public HitRollResultValidator() : base()
    {
    }
}
