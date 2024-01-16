using HotdogOrNot.ViewModels;

namespace HotdogOrNot.Views;

public partial class ResultView : ContentPage
{
	public ResultView(ResultViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}
