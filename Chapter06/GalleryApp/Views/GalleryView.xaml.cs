namespace GalleryApp.Views;

using GalleryApp.Models;
using GalleryApp.ViewModels;

public partial class GalleryView : ContentPage
{
	public GalleryView(GalleryViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        MainThread.InvokeOnMainThreadAsync(viewModel.Initialize);
    }

    private void SelectToolBarItem_Clicked(object sender, EventArgs e)
    {
        if (!Photos.SelectedItems.Any())
        {
            DisplayAlert("No photos", "No photos selected", "OK");
            return;
        }
        var viewModel = (GalleryViewModel)BindingContext; 
        viewModel.AddFavoritesCommand.Execute(Photos.SelectedItems.Select(x =>(Photo)x).ToList());

        DisplayAlert("Added", "Selected photos has been added to favorites", "OK");
    }
}