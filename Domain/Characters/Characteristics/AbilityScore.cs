using FluentValidation.Validators;

namespace Characters.Characteristics
{
    public class AbilityScore
    {
        public AbilityScore(AbilityEnum name, int value)
        {
            Name = name;
            Value = value;
        }

        public AbilityEnum Name { get; set; }
        
        public string ShortName => Name.ToString().Substring(0, 3).ToUpper();

        public int Value { get; set; }

        public bool HasMastery { get; set; } = false;

        public int GetModifier(int masteryBonus = 0) => (Value - 10) / 2 + (HasMastery ? masteryBonus : 0);

        public string GetModifierString(int masteryBonus)
        {
            var value = GetModifier(masteryBonus);
            return (value >= 0 ? "+" : "") + value;
        }
    }
}
