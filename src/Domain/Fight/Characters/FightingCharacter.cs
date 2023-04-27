using DnDEntities.Characters;

namespace Fight.Characters;

public class FightingCharacter
{
    public CharacterType CharacterType { get; init; }

    public Guid CharacterId { get; init; }


    public FightingCharacter(Character character)
    {
        CharacterType = character.Type;
        CharacterId = character.Id;
    }
}