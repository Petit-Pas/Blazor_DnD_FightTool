using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;

namespace DnDFightTool.Domain.DnDEntities.AbilityScores;

// TODO unit tests

/// <summary>
///     Collection of ability scores
///     Ex: Strength, Dexterity, ...
///     
///     Also holds the mastery bonus as it's often stored with such stats, might move it later.
/// </summary>
public class AbilityScoresCollection : List<AbilityScore>
{
    /// <summary>
    ///     default ctor
    /// </summary>
    public AbilityScoresCollection() : this(false)
    {
    }

    /// <summary>
    ///     ctor that can create default values
    /// </summary>
    /// <param name="withDefaults"> true if you want the default abilities to be populated </param>
    public AbilityScoresCollection(bool withDefaults = false)
    {
        if (withDefaults)
        {
            AddRange(AbilityEnumExtensions.All.Select(x => new AbilityScore(x)));
        }
    }

    /// <summary>
    ///     Returns the raw modifier for the given ability score
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ScoreModifier GetModifier(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Ability == name);

        if (ability == null)
        {
            // TODO warn
            return ScoreModifier.Empty;
        }

        return ability.GetModifier();
    }

    /// <summary>
    ///    Returns the modifier for the given ability score, with mastery bonus if applicable
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ScoreModifier GetSavingModifier(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Ability == name);

        if (ability == null)
        {
            // TODO warn
            return ScoreModifier.Empty;
        }

        return ability.GetSavingModifier(MasteryBonus);
    }

    public string ModifiersString(AbilityScore abilityScore)
    {
        return $"{GetModifier(abilityScore.Ability)}/{GetSavingModifier(abilityScore.Ability)}";
    }

    /// <summary>
    ///     The mastery bonus
    /// </summary>
    public int MasteryBonus { get; set; } = 2;
}