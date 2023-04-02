using System.Text.Json.Serialization;
using DnDEntities.DiceThrows.Modifiers;

namespace DnDEntities.AbilityScores;

public class AbilityScore
{
    public AbilityScore(AbilityEnum name, int score)
    {
        Name = name;
        Score = score;
    }

    public AbilityEnum Name { get; set; }

    [JsonIgnore]
    public string ShortName => Name.ToString().Substring(0, 3).ToUpper();

    public int Score { get; set; }

    public bool HasMastery { get; set; } = false;

    public ScoreModifier GetModifier(int masteryBonus = 0) => new ((Score - 10) / 2 + (HasMastery ? masteryBonus : 0));
}