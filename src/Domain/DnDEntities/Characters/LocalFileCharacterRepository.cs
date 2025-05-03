using IO.Files;
using IO.Serialization;

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
    private readonly IJsonSerializer _jsonSerializer;

    public LocalFileCharacterRepository(IFileManager fileManager, IJsonSerializer jsonSerializer)
    {
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
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

        foreach (var serializedCharacter in serializedCharacters)
        {
            var character = _jsonSerializer.Deserialize<Character>(serializedCharacter);
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
        if (_characters.TryGetValue(characterById, out var character))
        {
            return character;
        }
        return null;
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

        _fileManager.SaveAs(_jsonSerializer.Serialize(character), $"{character.Id}");
    }

    /// <inheritdoc />
    public int Count => _characters.Count;
}