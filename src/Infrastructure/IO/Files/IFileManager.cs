namespace IO.Files;

/// <summary>
///     Interface for a local file manager
/// </summary>
public interface IFileManager
{
    /// <summary>
    ///     Sets the main folder to use for all subsequent operations
    ///     Will also make sure that folder exists
    /// </summary>
    /// <param name="mainFolder"></param>
    void SetMainFolder(string mainFolder);

    /// <summary>
    ///     Makes sure the given directory exists
    ///     Will create it otherwise
    /// </summary>
    /// <param name="folderName"></param>
    void EnsureDirectoryExists(string folderName);

    /// <summary>
    ///     Read all files in the given folder as strings.
    /// </summary>
    /// <param name="subFolder"> a non required subfolder </param>
    /// <returns></returns>
    IEnumerable<string> ReadAllFiles(string subFolder = "");

    /// <summary>
    ///     Saves a given message to a given file
    /// </summary>
    /// <param name="message"></param>
    /// <param name="filename"></param>
    void SaveAs(string message, string filename);

    /// <summary>
    ///     Deletes a given file
    /// </summary>
    /// <param name="filename"></param>
    void Delete(string filename);
}
