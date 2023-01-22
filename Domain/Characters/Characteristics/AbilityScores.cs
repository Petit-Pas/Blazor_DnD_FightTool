using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Characters.Characteristics;

public class AbilityScores : List<AbilityScore>
{
    public AbilityScores()
    {
        this.AddRange(new AbilityScore[]
        {
            new (AbilityEnum.Strength, 10) {HasMastery = true},
            new (AbilityEnum.Dexterity, 10),
            new (AbilityEnum.Constitution, 10),
            new (AbilityEnum.Intelligence, 10),
            new (AbilityEnum.Wisdom, 10),
            new (AbilityEnum.Charisma, 10),
        });
    }

    public int GetModifierWithoutMastery(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Name == name);

        if (ability == null)
        {
            // TODO warn
            return 0;
        }

        return ability.GetModifier();
    }

    public int GetModifierWithMastery(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Name == name);

        if (ability == null)
        {
            // TODO warn
            return 0;
        }

        return ability.GetModifier(MasteryBonus);
    }

    public string GetModifierStringWithoutMastery(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Name == name);

        if (ability == null)
        {
            // TODO warn
            return "+0";
        }

        return ability.GetModifierString(MasteryBonus);
    }

    public string GetModifierStringWithMastery(AbilityEnum name)
    {
        var ability = this.FirstOrDefault(x => x.Name == name);

        if (ability == null)
        {
            // TODO warn
            return "+0";
        }

        return ability.GetModifierString(MasteryBonus);
    }

    public int MasteryBonus { get; set; } = 2;
}