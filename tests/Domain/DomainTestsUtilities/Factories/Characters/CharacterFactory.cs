using DnDFightTool.Domain.DnDEntities.Characters;

namespace DomainTestsUtilities.Factories.Characters;

public static class CharacterFactory
{
    public static Character BuildMonster(Guid? id = null, string? name = null)
    {
        return BuildCharacter(CharacterType.Monster, id, name);
    }

    public static Character BuildPlayer(Guid? id = null, string? name = null)
    {
        return BuildCharacter(CharacterType.Player, id, name);
    }

    private static Character BuildCharacter(CharacterType characterType, Guid? id = null, string? name = null)
    {
        return new Character()
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? "character name",
            Type = characterType
        };
    }
}
