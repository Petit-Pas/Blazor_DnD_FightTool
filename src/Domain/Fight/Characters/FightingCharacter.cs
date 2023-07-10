using DnDEntities.Characters;

namespace Fight.Characters;

public record FightingCharacter(string Name, CharacterType CharacterType, Guid CharacterId)
{
	public FightingCharacter(Character character) : this(character.Name, character.Type, character.Id)
	{
	}
}