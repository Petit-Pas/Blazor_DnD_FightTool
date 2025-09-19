using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.DnDEntities.Dices.DiceThrows;

namespace DnDEntitiesBlazorComponents.DnDEntities.Dices;

/// <summary>
///     Class to contain UI related extension methods for <see cref="DiceThrowTemplate" />"/>
/// </summary>
public static class DiceThrowTemplateExtensions
{
    public static string GetRangeDescription(this DiceThrowTemplate template, ICharacter? caster)
    {
        return caster is not null ? $"({template.MinimumResult(caster)}-{template.MaximumResult(caster)})" : string.Empty;
    }
}
