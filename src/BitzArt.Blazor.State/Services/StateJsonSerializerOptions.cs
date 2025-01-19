using System.Text.Json;
using System.Text.Json.Serialization;

namespace BitzArt.Blazor.State;

internal class StateJsonSerializerOptions
{
    public readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
