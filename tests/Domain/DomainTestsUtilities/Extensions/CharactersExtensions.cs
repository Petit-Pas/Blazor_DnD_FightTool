using DnDFightTool.Domain.DnDEntities.Characters;
using DnDFightTool.Domain.Fight.Characters;

namespace DomainTestsUtilities.Extensions;

public static class CharactersExtensions
{
    public static FightingCharacter AsFighter(this Character character)
    {
        return new FightingCharacter(character);
    }
}
