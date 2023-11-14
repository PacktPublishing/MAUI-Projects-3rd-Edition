namespace GalleryApp.Services;

using System.Text.Json;

public class MauiLocalStorage : ILocalStorage
{
    public const string FavoritePhotosKey = "FavoritePhotos";

    public List<string> Get()
    {
        if (Preferences.ContainsKey(FavoritePhotosKey))
        {
            var filenames = Preferences.Get(FavoritePhotosKey,string.Empty);
            return JsonSerializer.Deserialize<List<string>>(filenames);
        }
        return new List<string>();
    }

    public void Store(string filename)
    {
        var filenames = Get(); 
        filenames.Add(filename);

        var json = JsonSerializer.Serialize(filenames);

        Preferences.Set(FavoritePhotosKey, json); 
    }
}
