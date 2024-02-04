using System.Text.Json;
using System.Text.Json.Serialization;
using IO.Files;

namespace DnDFightTool.Domain.DnDEntities.Characters;

/// <summary>
///     Local implementation of ICharacterRepository
/// </summary>
public class LocalFileCharacterRepository : ICharacterRepository
{
    // Note that this path will be transformed on windows on C/user/*username*/appdata/local/package/*app_uid*_suffix/localappdata... 
    private readonly static string _mainFolder = Environment.GetEnvironmentVariable("LocalAppData") + @"\DnDFightTool";

    private readonly Dictionary<Guid, Character> _characters;
    private readonly IFileManager _fileManager;

    public LocalFileCharacterRepository(IFileManager fileManager)
    {
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));

        _characters = LoadCharacters();
    }

    /// <summary>
    ///     Private method used by the constructor to load all characters on start
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Dictionary<Guid, Character> LoadCharacters()
    {
        var characters = new Dictionary<Guid, Character>();
        
        var serializedCharacters = ReadAllSaveFiles();

        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(new JsonStringEnumConverter());

        foreach (var serializedCharacter in serializedCharacters)
        {
            var character = JsonSerializer.Deserialize<Character>(serializedCharacter, serializerOptions);
            if (character == null)
            {
                // TODO error handling should be better
                // TODO warn 
                throw new Exception("Could not deserialize character");
            }
            else
            {
                characters[character.Id] = character;
            }
        }

        return characters;
    }

    /// <summary>
    ///     
    /// </summary>
    /// <returns></returns>
    private IEnumerable<string> ReadAllSaveFiles()
    {
        _fileManager.SetMainFolder(_mainFolder);
        return _fileManager.ReadAllFiles();
    }

    /// <inheritdoc />
    public Character? GetCharacterById(Guid characterById)
    {
        if (!_characters.ContainsKey(characterById))
        {
            return null;
        }

        return _characters[characterById];
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Character> GetAllCharacters()
    {
        return _characters.Values;
    }

    /// <inheritdoc />
    public void Delete(Character character)
    {
        _fileManager.Delete(character.Id.ToString());
        _characters.Remove(character.Id);
    }

    /// <inheritdoc />
    public void Save(Character character)
    {
        _characters[character.Id] = character;
        
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(new JsonStringEnumConverter());

        _fileManager.SaveAs(JsonSerializer.Serialize(character, serializerOptions), $"{character.Id}");
    }

    /// <inheritdoc />
    public int Count => _characters.Count;
}