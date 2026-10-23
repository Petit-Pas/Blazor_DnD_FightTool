using DnDFightTool.Domain.CharacterSheet.Characters;

namespace DomainTestsUtilities.Factories.Characters;

public static class CharacterFactory
{
    public static Character BuildMonster(Guid? id = null, string? name = null, bool withDefaults = false)
    {
        return BuildCharacter(CharacterType.Monster, id, name, withDefaults);
    }

    public static Character BuildPlayer(Guid? id = null, string? name = null, bool withDefaults = false)
    {
        return BuildCharacter(CharacterType.Player, id, name, withDefaults);
    }

    private static Character BuildCharacter(CharacterType characterType, Guid? id, string? name, bool withDefaults)
    {
        return new Character(withDefaults)
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "character name",
            Type = characterType
        };
    }
}
