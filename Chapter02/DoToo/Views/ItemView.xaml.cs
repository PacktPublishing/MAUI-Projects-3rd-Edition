namespace DoToo.Views;

using DoToo.ViewModels;

public partial class ItemView : ContentPage
{
	public ItemView(ItemViewModel viewModel)
    {
        InitializeComponent();
        viewModel.Navigation = Navigation;
        BindingContext = viewModel;
    }
}