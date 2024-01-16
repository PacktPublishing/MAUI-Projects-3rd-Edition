using HotdogOrNot.ViewModels;

namespace HotdogOrNot.Views;

public partial class MainView : ContentPage
{
	public MainView(MainViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
		NavigationPage.SetBackButtonTitle(this, string.Empty);
    }
}
