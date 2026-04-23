using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.CharacterSheet.Statuses;
using DnDFightTool.Domain.Rolls;

namespace DnDFightTool.Domain.Fight.DomainExtensions.Statuses;

/// <summary>
///     Extension methods for <see cref="StatusTemplate"/> that depend on roll results.
/// </summary>
public static class StatusTemplateExtensions
{
    /// <summary>
    ///    Tells whether the status should be applied or not
    /// </summary>
    /// <param name="statusTemplate"></param>
    /// <param name="caster"></param>
    /// <param name="target"></param>
    /// <param name="saveRoll"></param>
    /// <returns></returns>
    public static bool ShouldBeApplied(this StatusTemplate statusTemplate, ICharacter caster, ICharacter target, SaveRollResult? saveRoll)
    {
        if (statusTemplate.IsAppliedAutomatically || !(saveRoll?.IsSuccessful(caster, target) ?? false))
        {
            return true;
        }
        return false;
    }
}
