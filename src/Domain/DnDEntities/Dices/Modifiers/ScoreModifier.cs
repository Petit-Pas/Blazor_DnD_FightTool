using System.Numerics;

namespace DnDEntities.Dices.Modifiers;

// TODO not certain this class is in the right place

public record ScoreModifier(int Modifier)
{
    public static readonly ScoreModifier Empty = new (0);

    public string ModifierString => (Modifier >= 0 ? "+" : "") + Modifier;
}
