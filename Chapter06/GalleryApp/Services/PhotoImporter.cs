namespace GalleryApp.Services;

using GalleryApp.Models;
using System.Collections.ObjectModel;

internal partial class PhotoImporter : IPhotoImporter
{
    private partial Task<string[]> Import();

    public partial Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality);

    public partial Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality);

}