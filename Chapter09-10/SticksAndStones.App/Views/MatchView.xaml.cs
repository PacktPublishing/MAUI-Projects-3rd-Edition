using SticksAndStones.ViewModels;

namespace SticksAndStones.Views;

public partial class MatchView : ContentPage
{
	public MatchView(MatchViewModel viewModel)
	{
		this.BindingContext = viewModel;
		InitializeComponent();
	}
}