using DnDEntities.Characters;

namespace Fight.Characters;

public record FightingCharacter(CharacterType CharacterType, Guid CharacterId)
{
	public FightingCharacter(Character character) : this(character.Type, character.Id)
	{
	}
}