using News.ViewModels;

namespace News.Views;

public partial class HeadlinesView : ContentPage
{
    readonly HeadlinesViewModel viewModel;

	public HeadlinesView(HeadlinesViewModel viewModel)
	{
        this.viewModel = viewModel;
		InitializeComponent();
        Task.Run(async () => await Initialize(GetScopeFromRoute()));
    }

    private async Task Initialize(string scope)
    {
        BindingContext = viewModel;
        await viewModel.Initialize(scope);
    }

    private string GetScopeFromRoute()
    {
        // Hack: As the shell can't define query parameters
        // in XAML, we have to parse the route. 
        // as a convention the last route section defines the category.
        var route = Shell.Current.CurrentState.Location
            .OriginalString.Split("/").LastOrDefault();
        return route;
    }
}