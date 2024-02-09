using SticksAndStones.ViewModels;

namespace SticksAndStones.Views;

public partial class ConnectView : ContentPage
{
    public ConnectView(ConnectViewModel viewModel)
    {
        this.BindingContext = viewModel;

        InitializeComponent();
    }
}