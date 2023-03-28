using Fight.AttackRolls.ArmorClasses;

namespace Characters.Characters;

public class Character
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = "";

    public CharacterType Type { get; set; } = CharacterType.Unknown;

    public ArmorClass ArmorClass { get; set; } = new();

}