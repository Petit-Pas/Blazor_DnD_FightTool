using DnDFightTool.Domain.DnDEntities.Characters;
using Memory.Hashes;

namespace DnDFightTool.Domain.DnDEntities.Damage;

/// <summary>
///    A collection of <see cref="DamageRollTemplate"/>
/// </summary>
public class DamageRollTemplateCollection : List<DamageRollTemplate>, IHashable
{
    /// <summary>
    ///     Gets the minimum rollable result, so dice + modifiers
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public int MinimumResult(Character character)
    {
        return this.Sum(dmg => dmg.Dices.MinimumResult(character));
    }

    /// <summary>
    ///     Gets the maximum rollable result, so dice + modifiers
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public int MaximumResult(Character character)
    {
        return this.Sum(dmg => dmg.Dices.MaximumResult(character));
    }
}
