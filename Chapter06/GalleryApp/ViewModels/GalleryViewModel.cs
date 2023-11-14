namespace GalleryApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GalleryApp.Models;
using GalleryApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public partial class GalleryViewModel : ViewModel
{
    private readonly IPhotoImporter photoImporter;

    [ObservableProperty]
    public ObservableCollection<Photo> photos;

    [ObservableProperty]
    public ILocalStorage localStorage;

    public GalleryViewModel(IPhotoImporter photoImporter, ILocalStorage localStorage) : base()
    {
        this.photoImporter = photoImporter; 
        this.localStorage = localStorage; 
    }

    override protected internal async Task Initialize()
    {
        IsBusy = true;
        Photos = await photoImporter.Get(0, 20); 

        Photos.CollectionChanged += Photos_CollectionChanged;
        await Task.Delay(3000);
        IsBusy = false;
    }

    private void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null && e.NewItems.Count > 0)
        {
            IsBusy = false;
            Photos.CollectionChanged -= Photos_CollectionChanged;
        }
    }

    private int currentStartIndex = 0;

    [RelayCommand]
    public async Task LoadMore()
    {
        currentStartIndex += 20;
        itemsAdded = 0;
        var collection = await photoImporter.Get(currentStartIndex, 20);
        collection.CollectionChanged += Collection_CollectionChanged;
    }

    private int itemsAdded;
    private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
    {
        foreach (Photo photo in args.NewItems)
        {
            itemsAdded++; 
            Photos.Add(photo);
        }
        if (itemsAdded == 20)
        {
            var collection = (ObservableCollection<Photo>)sender; 
            collection.CollectionChanged -= Collection_CollectionChanged;
        }
    }

    [RelayCommand]
    public void AddFavorites(List<Photo> photos)
    {
        foreach (var photo in photos)
        {
            localStorage.Store(photo.Filename);
        }
        WeakReferenceMessenger.Default.Send<string>(Messages.FavoritesAddedMessage);
    }
}