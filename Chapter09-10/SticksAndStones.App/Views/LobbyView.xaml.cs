using SticksAndStones.ViewModels;

namespace SticksAndStones.Views;

public partial class LobbyView : ContentPage
{
    public LobbyView(LobbyViewModel viewModel)
    {
        this.BindingContext = viewModel;
        InitializeComponent();
    }
}