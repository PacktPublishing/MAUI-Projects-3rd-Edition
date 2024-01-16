using System.Text.Json.Serialization;

namespace SticksAndStones;

public record AsyncError
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
}

public record AsyncExceptionError : AsyncError
{
    [JsonPropertyName("innerException")]
    public string InnerException { get; set; }
}
