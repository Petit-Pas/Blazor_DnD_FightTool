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

    /// <summary>
    ///     An additional amount to add to the saving throw for this ability
    /// </summary>
    public int ArbitrarySaveModifier { get; set; } = 0;

    /// <summary>
    ///     If this ability is mastered
    /// </summary>
    public bool HasMastery { get; set; }

    /// <summary>
    ///     Gets the modifier for this ability score
    ///     eg: 
    ///         - 10 => 0, 
    ///         - 16 => 3, 
    ///         - 8 => -1
    /// </summary>
    /// <param name="masteryBonus"></param>
    /// <returns></returns>
    public ScoreModifier GetModifier()
    {
        return Score / 2 - 5;
    }

    /// <summary>
    ///     Gets the modifier for this ability score, with the potential mastery bonus applied if necessary
    /// </summary>
    /// <param name="masteryBonus"></param>
    /// <returns></returns>
    public ScoreModifier GetSavingModifier(int masteryBonus)
    {
        return GetModifier() + (HasMastery ? masteryBonus : 0) + ArbitrarySaveModifier;
    }
}