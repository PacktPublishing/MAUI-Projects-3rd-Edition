namespace GalleryApp.Views;

using GalleryApp.ViewModels;

public partial class MainView : ContentPage
{
    public MainView(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        MainThread.InvokeOnMainThreadAsync(viewModel.Initialize);
    }
}