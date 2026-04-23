using DnDFightTool.Domain.CharacterSheet.Saves;

namespace DnDFightTool.Domain.Rolls.Extensions;

/// <summary>
///     Extension methods for converting save roll templates to roll results.
/// </summary>
public static class SaveRollTemplateExtensions
{
    /// <summary>
    ///     Creates an empty <see cref="SaveRollResult"/> from this template.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static SaveRollResult GetEmptyRollResult(this SaveRollTemplate template)
    {
        return new SaveRollResult(template.Difficulty, template.TargetAbility);
    }
}
