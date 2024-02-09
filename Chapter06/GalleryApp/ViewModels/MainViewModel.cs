namespace GalleryApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GalleryApp.Models;
using GalleryApp.Services;
using System.Collections.ObjectModel;

public partial class MainViewModel : ViewModel
{ 
    private readonly IPhotoImporter photoImporter; 
    private readonly ILocalStorage localStorage;

    [ObservableProperty]
    private ObservableCollection<Photo> recent;

    [ObservableProperty]
    private ObservableCollection<Photo> favorites;

    public MainViewModel(IPhotoImporter photoImporter, ILocalStorage localStorage)
    {
        this.photoImporter = photoImporter; 
        this.localStorage = localStorage;
    }

    override protected internal async Task Initialize()
    {
        var photos = await photoImporter.Get(0, 20, Quality.Low);

        Recent = photos; 
        await LoadFavorites();

        WeakReferenceMessenger.Default.Register<string>(this, async (sender, message) => {
            if( message == Messages.FavoritesAddedMessage )
            {
                await MainThread.InvokeOnMainThreadAsync(LoadFavorites);
            } 
        });
    }

    private async Task LoadFavorites()
    {
        var filenames = localStorage.Get();
        var favorites = await photoImporter.Get(filenames, Quality.Low);

        Favorites = favorites; 
    }
}

