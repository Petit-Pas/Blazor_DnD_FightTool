namespace DnDFightTool.Domain.DnDEntities.Characters;

public interface ICharacterRepository 
{
    Character? GetCharacterById(Guid characterById);

    Character? GetCharacterByIndex(int index);

    IEnumerable<Character> GetAllCharacters();
    IEnumerable<Character> GetCharacters(Func<Character, bool> filter);

    void Delete(Character character);

    void Save(Character character);

    int Count { get; }
}