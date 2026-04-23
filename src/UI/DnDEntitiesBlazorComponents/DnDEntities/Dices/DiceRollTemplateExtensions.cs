using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices;

namespace DnDEntitiesBlazorComponents.DnDEntities.Dices;

/// <summary>
///     Class to contain UI related extension methods for <see cref="DiceRollTemplate" />"/>
/// </summary>
public static class DiceRollTemplateExtensions
{
    public static string GetRangeDescription(this DiceRollTemplate template, ICharacter? caster)
    {
        return caster is not null ? $"({template.MinimumResult(caster)}-{template.MaximumResult(caster)})" : string.Empty;
    }
}
