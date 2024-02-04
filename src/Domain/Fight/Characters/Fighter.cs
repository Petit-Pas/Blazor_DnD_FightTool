using DnDFightTool.Domain.DnDEntities.Characters;

namespace DnDFightTool.Domain.Fight.Characters;

/// <summary>
///     Record for the minimal required information of a fighter.
///     Does not hold the information of the character itself, only the information required to identify it.
/// </summary>
/// <param name="Name"></param>
/// <param name="CharacterType"></param>
/// <param name="CharacterId"></param>
public record Fighter(string Name, CharacterType CharacterType, Guid CharacterId)
{
    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="character"></param>
	public Fighter(Character character) : this(character.Name, character.Type, character.Id)
	{
	}
}