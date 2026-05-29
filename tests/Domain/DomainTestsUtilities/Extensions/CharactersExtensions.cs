using DnDFightTool.Domain.CharacterSheet.Characters;
using DnDFightTool.Domain.Fight.Characters;

namespace DomainTestsUtilities.Extensions;

public static class CharactersExtensions
{
    public static FightingCharacter AsFighter(this Character character, Guid? originalCharacterId = null)
    {
        return new FightingCharacter(character, originalCharacterId ?? character.Id);
    }
}
