using DnDFightTool.Domain.DnDEntities.Dices.Modifiers;

namespace DnDFightTool.Domain.DnDEntities.AbilityScores;

public class AbilityScore
{
    public AbilityScore(AbilityEnum ability, int score)
    {
        Ability = ability;
        Score = score;
    }

    public AbilityEnum Ability { get; set; }

    public int Score { get; set; }

    public bool HasMastery { get; set; } = false;

    public ScoreModifier GetModifier(int masteryBonus = 0) => new ScoreModifier(Score / 2 - 5 + (HasMastery ? masteryBonus : 0));
}