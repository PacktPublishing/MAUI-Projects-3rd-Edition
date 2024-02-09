namespace GalleryApp.Services;

using GalleryApp.Models;
using Android.Provider;
using System.Collections.ObjectModel;

internal partial class PhotoImporter 
{
    private partial async Task<string[]> Import()
    {
        var paths = new List<string>();

        var status = await AppPermissions.CheckAndRequestRequiredPermission();
        if (status == PermissionStatus.Granted)
        {

            var imageUri = MediaStore.Images.Media.ExternalContentUri;
            var projection = new string[] { MediaStore.IMediaColumns.Data };
            //var selection = new string[] { "image/jpeg", "image/png" };
            var orderBy = MediaStore.Images.IImageColumns.DateTaken;
            var cursor = Platform.CurrentActivity.ContentResolver.Query(imageUri, projection, /*MediaStore.IMediaColumns.MimeType, selection*/ null, null, orderBy);
            while (cursor.MoveToNext())
            {
                string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.IMediaColumns.Data));
                paths.Add(path);
            }
        }
        return paths.ToArray();
    }

    public partial async Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality)
    {
        var photos = new ObservableCollection<Photo>();

        var result = await Import();

        if (result.Length == 0)
        {
            return photos;
        }

        Index startIndex = start;
        Index endIndex = start + count;

        if (endIndex.Value >= result.Length)
        {
            endIndex = result.Length;
        }
        if (startIndex.Value > endIndex.Value)
        {
            return photos;
        }

        foreach (var path in result[startIndex..endIndex])
        {
            photos.Add(new()
            {
                Bytes = File.ReadAllBytes(path),
                Filename = Path.GetFileName(path)
            });
        }

        return photos;
    }

    public partial async Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality)
    {
        var photos = new ObservableCollection<Photo>();

        var result = await Import();

        if (result.Length == 0)
        {
            return photos;
        }

        foreach (var path in result)
        {
            var filename = Path.GetFileName(path);

            if (!filenames.Contains(filename))
            {
                continue;
            }

            photos.Add(new Photo()
            {
                Bytes = File.ReadAllBytes(path),
                Filename = filename
            });
        }

        return photos;
    }
}
