namespace Fight.DiceThrows.Modifiers
{
    public record ScoreModifier(int Modifier)
    {
        public static readonly ScoreModifier Empty = new (0);

        public string ModifierString => (Modifier >= 0 ? "+" : "") + Modifier;
    }
}
