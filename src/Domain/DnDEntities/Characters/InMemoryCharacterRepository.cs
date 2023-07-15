using System.Text.Json;
using System.Text.Json.Serialization;

namespace DnDFightTool.Domain.DnDEntities.Characters;

public interface IFileManager
{
    void SaveAs(string message, string filename);
    IEnumerable<string> LoadAllCharacters();
    void Delete(string filename);
}

public class FileManager : IFileManager
{
    // Note that this path will be transformed on windows on C/user/*username*/appdata/local/package/*app_uid*_suffix/localappdata... 
    private static readonly string _mainFolder = Environment.GetEnvironmentVariable("LocalAppData") + @"\DnDFightTool";

    private void EnsureDirectoryExists(string folderName)
    {
        if (folderName != null && !Directory.Exists(folderName))
            Directory.CreateDirectory(folderName);
    }

    public IEnumerable<string> LoadAllCharacters()
    {
        EnsureDirectoryExists(_mainFolder);

        foreach (var file in Directory.GetFiles(_mainFolder))
        {
            using var reader = new StreamReader(file);
            yield return reader.ReadToEnd();
        }
    }

    public void Delete(string filename)
    {
        File.Delete($@"{_mainFolder}\{filename}.json");
    }

    public void SaveAs(string message, string filename)
    {
        EnsureDirectoryExists(_mainFolder);

        StreamWriter writer = new StreamWriter($@"{_mainFolder}\{filename}.json");
        writer.Write(message);
        writer.Close();
    }
}

public class InMemoryCharacterRepository : ICharacterRepository
{
    private Dictionary<Guid, Character> _characters;
    private IFileManager _fileManager;

    public InMemoryCharacterRepository(IFileManager fileManager)
    {
        _fileManager = fileManager;

        _characters = new Dictionary<Guid, Character>();

        var serializedCharacters = _fileManager.LoadAllCharacters();
        
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        
        foreach (var serializedCharacter in serializedCharacters)
        {
            var character = JsonSerializer.Deserialize<Character>(serializedCharacter, serializerOptions);
            _characters[character.Id] = character;
        }
    }

    public Character? GetCharacterById(Guid characterById)
    {
        if (!_characters.ContainsKey(characterById))
        {
            return null;
        }

        return _characters[characterById];
    }

    public Character? GetCharacterByIndex(int index)
    {
        if (_characters.Count <= index)
        {
            return null;
        }

        return _characters.ElementAt(index).Value;
    }

    public IEnumerable<Character> GetAllCharacters()
    {
        return _characters.Values;
    }

    public IEnumerable<Character> GetCharacters(Func<Character, bool> filter)
    {
        return _characters.Values.Where(filter);
    }

    public void Delete(Character character)
    {
        _fileManager.Delete(character.Id.ToString());
        _characters.Remove(character.Id);
    }

    public void Save(Character character)
    {
        _characters[character.Id] = character;
        
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(new JsonStringEnumConverter());

        _fileManager.SaveAs(JsonSerializer.Serialize(character, serializerOptions), $"{character.Id}");
    }

    public int Count => _characters.Count;
}