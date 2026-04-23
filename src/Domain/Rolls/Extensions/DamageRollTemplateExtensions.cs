using DnDFightTool.Domain.CharacterSheet.Damage;

namespace DnDFightTool.Domain.Rolls.Extensions;

/// <summary>
///     Extension methods for converting damage roll templates to roll results.
/// </summary>
public static class DamageRollTemplateExtensions
{
    /// <summary>
    ///     Creates an empty <see cref="DamageRollResult"/> from this template.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static DamageRollResult GetEmptyRollResult(this DamageRollTemplate template)
    {
        return new DamageRollResult(template.Dices, template.Type);
    }

    /// <summary>
    ///     Creates rollable damage results from a collection of templates.
    /// </summary>
    /// <param name="damageRollTemplateCollection"></param>
    /// <returns></returns>
    public static DamageRollResult[] GetRollableResult(this DamageRollTemplateCollection damageRollTemplateCollection)
    {
        return [.. damageRollTemplateCollection.Select(x => x.GetEmptyRollResult())];
    }
}
