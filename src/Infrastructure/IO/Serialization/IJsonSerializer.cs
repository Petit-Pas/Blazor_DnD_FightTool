namespace IO.Serialization;

/// <summary>
///     Wrapper around System.Text.Json.JsonSerializer
/// </summary>
public interface IJsonSerializer
{
    /// <summary>
    ///     Serializes the object to a string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="toSerialize"></param>
    /// <returns></returns>
    string Serialize<T>(T toSerialize);

    /// <summary>
    ///     Deserializes the string to the requested type object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="toDeserialize"></param>
    /// <returns></returns>
    T? Deserialize<T>(string toDeserialize);
}
