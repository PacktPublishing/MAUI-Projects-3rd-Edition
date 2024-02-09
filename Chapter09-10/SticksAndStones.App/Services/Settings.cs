using SticksAndStones.Models;
using System.Text.Json;

namespace SticksAndStones.Services;

public class Settings
{
    private const string LastPlayerKey = nameof(LastPlayerKey);
    private const string ServerUrlKey = nameof(ServerUrlKey);

#if DEBUG && ANDROID
    private const string ServerUrlDefault = "http://10.0.2.2:7071/api";
#else
    private const string ServerUrlDefault = "http://localhost:7071/api";
#endif

    public string ServerUrl
    {
        get => Preferences.ContainsKey(ServerUrlKey) ?
                    Preferences.Get(ServerUrlKey, ServerUrlDefault) :
                    ServerUrlDefault;
        set => Preferences.Set(ServerUrlKey, value);
    }

    public Player LastPlayer
    {
        get
        {
            if (Preferences.ContainsKey(LastPlayerKey))
            {
                var playerJson = Preferences.Get(LastPlayerKey, string.Empty);
                return JsonSerializer.Deserialize<Player>(playerJson, new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? new();
            }
            return new();
        }
        set => Preferences.Set(LastPlayerKey, JsonSerializer.Serialize(value, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
    }

}
