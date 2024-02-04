namespace DnDFightTool.Domain.DnDEntities.Characters;

/// <summary>
///     A repository for all characters
///     Note that these are not the fighting ones, so monsters for instance would always be full HP in here.
/// </summary>
public interface ICharacterRepository 
{
    /// <summary>
    ///     Gets a character by its unique non meaningful identifier
    /// </summary>
    /// <param name="characterById"></param>
    /// <returns></returns>
    Character? GetCharacterById(Guid characterById);

    /// <summary>
    ///    Gets all the characters in the repository
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<Character> GetAllCharacters();

    /// <summary>
    ///     Deletes a specific character from the repository
    /// </summary>
    /// <param name="character"></param>
    void Delete(Character character);

    /// <summary>
    ///     Saves a specific character to the repository
    ///     Will override the previous version of the character (by unique non meaningful id, if it exists)
    /// </summary>
    /// <param name="character"></param>
    void Save(Character character);

    /// <summary>
    ///     The amount of records in the repository
    /// </summary>
    int Count { get; }
}