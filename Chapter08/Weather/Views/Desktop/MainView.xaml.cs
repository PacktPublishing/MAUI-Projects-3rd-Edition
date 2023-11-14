using Weather.ViewModels;

namespace Weather.Views.Desktop;

public partial class MainView : ContentPage, IMainView
{
    public MainView(MainViewModel mainViewModel)
    {
        InitializeComponent();

        BindingContext = mainViewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is MainViewModel viewModel)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await viewModel.LoadDataAsync();
            });
        }
    }
}
