using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;

namespace DnDFightTool.Domain.DnDEntities.AbilityScores;

/// <summary>
///    An ability score such as Strength, Dexterity, ...
/// </summary>
public class AbilityScore
{
    /// <summary>
    ///     Should not be used, only for deserialization
    /// </summary>
    [Obsolete("Should not be used, only for deserialization")]
    public AbilityScore()
    {
    }

    /// <summary>
    ///     Ctor with default values for the ability
    /// </summary>
    /// <param name="ability"></param>
    public AbilityScore(AbilityEnum ability) : this(ability, 10, false)
    {
    }

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="score"></param>
    public AbilityScore(AbilityEnum ability, int score) : this(ability, score, false)
    {
    }

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="score"></param>
    /// <param name="hasMastery"></param>
    public AbilityScore(AbilityEnum ability, int score, bool hasMastery)
    {
        Ability = ability;
        Score = score;
        HasMastery = hasMastery;
    }

    /// <summary>
    ///     The ability to which this ability score corresponds
    /// </summary>
    public AbilityEnum Ability { get; set; }

    /// <summary>
    ///     The score (1-30) for this ability
    /// </summary>
    public int Score { get; set; }

    // TODO maybe this information should not be there, the mastery is a separated concept. Should maybe be moved when we implement the fact that a save can have a default additional modifier (ring/cloak of protection)
    /// <summary>
    ///     If this ability is mastered
    /// </summary>
    public bool HasMastery { get; set; }

    /// <summary>
    ///     Gets the modifier for this ability score (with mastery bonus if applicable and provided)
    ///     eg: 
    ///         - 10 => 0, 
    ///         - 16 => 3, 
    ///         - 8 => -1
    /// </summary>
    /// <param name="masteryBonus"></param>
    /// <returns></returns>
    public ScoreModifier GetModifier(int masteryBonus = 0)
    {
        return new(Score / 2 - 5 + (HasMastery ? masteryBonus : 0));
    }
}