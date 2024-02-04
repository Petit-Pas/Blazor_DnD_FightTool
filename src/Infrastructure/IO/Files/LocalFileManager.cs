namespace IO.Files;

/// <summary>
///     Implementation of IFileManager for local files
/// </summary>
public class LocalFileManager : IFileManager
{
    private string _mainFolder = "";

    /// <inheritdoc />
    public void SetMainFolder(string mainFolder)
    {
        _mainFolder = mainFolder;
        EnsureDirectoryExists(_mainFolder);
    }

    /// <summary>
    ///     Helper method to append main folder to a a potential subfolder with '\
    /// </summary>
    /// <param name="subFolder"></param>
    /// <returns></returns>
    private string AppendMainFolder(string subFolder = "")
    {
        return Path.Combine(_mainFolder, subFolder);
    }

    /// <inheritdoc />
    public void EnsureDirectoryExists(string folderName)
    {
        if (!string.IsNullOrWhiteSpace(folderName) && !Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> ReadAllFiles(string subFolder = "")
    {
        foreach (var file in Directory.GetFiles(AppendMainFolder(subFolder)))
        {
            using var reader = new StreamReader(file);
            yield return reader.ReadToEnd();
        }
    }

    /// <inheritdoc />
    public void SaveAs(string message, string filename)
    {
        using var writer = new StreamWriter(AppendMainFolder(filename));
        writer.Write(message);
    }

    /// <inheritdoc />
    public void Delete(string filename)
    {
        File.Delete(AppendMainFolder(filename));
    }
}
