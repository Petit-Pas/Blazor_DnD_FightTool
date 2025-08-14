using System.Text.Json;
using System.Text.Json.Serialization;

namespace IO.Serialization;

/// <summary>
///     Implementation of IJsonSerializer
/// </summary>
public class JsonSerializer : IJsonSerializer
{
    /// <summary>
    ///     Serialization options
    /// </summary>
    private readonly JsonSerializerOptions _options;

    /// <summary>
    ///     Default constructor
    /// </summary>
    public JsonSerializer()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new JsonStringEnumConverter());
    }

    /// <inheritdoc/>
    public T? Deserialize<T>(string toDeserialize)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(toDeserialize, _options);
    }

    /// <inheritdoc/>
    public string Serialize<T>(T toSerialize)
    {
        return System.Text.Json.JsonSerializer.Serialize(toSerialize, _options);
    }
}
