namespace GalleryApp.Services;

using Foundation;
using GalleryApp.Models;
using Photos;
using System.Collections.ObjectModel;

internal partial class PhotoImporter
{
    private Dictionary<string,PHAsset> assets;

    private partial async Task<string[]> Import()
    {
        var status = await AppPermissions.CheckAndRequestRequiredPermission();
        if (status == PermissionStatus.Granted)
        {

            assets = PHAsset.FetchAssets(PHAssetMediaType.Image, null)
            .Select(x => (PHAsset)x)
            .ToDictionary(asset => asset.ValueForKey((NSString)"filename").ToString(), asset => asset);
        }
        return await Task.FromResult(assets?.Keys.ToList().ToArray());
    }

    public partial async Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality)
    {
        var photos = new ObservableCollection<Photo>();

        var result = await Import();
        if (result?.Length == 0)
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
            AddImage(photos, path, assets[path], quality);
        }
        return photos;
    }

    public partial async Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality)
    {
        var photos = new ObservableCollection<Photo>();

        var result = await Import();
        if (result?.Length == 0)
        {
            return photos;
        }

        foreach (var path in result) 
        {
            if (filenames.Contains(path))
            {
                AddImage(photos, path, assets[path], quality);
            }
        }

        return photos;
    }

    private void AddImage(ObservableCollection<Photo> photos, string path, PHAsset asset, Quality quality)
    {
        var options = new PHImageRequestOptions()
        {
            NetworkAccessAllowed = true,
            DeliveryMode = quality == Quality.Low ?
                PHImageRequestOptionsDeliveryMode.FastFormat :
                PHImageRequestOptionsDeliveryMode.HighQualityFormat
        };

        PHImageManager.DefaultManager.RequestImageForAsset(asset, PHImageManager.MaximumSize, PHImageContentMode.AspectFill, options, (image, info) =>
        {
            using NSData imageData = image.AsPNG();
            var bytes = new byte[imageData.Length]; 
            System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, bytes, 0, Convert.ToInt32(imageData.Length));
            photos.Add(new Photo()
            {
                Bytes = bytes,
                Filename = Path.GetFileName(path)
            });
        });
    }
}
