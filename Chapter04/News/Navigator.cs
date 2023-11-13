namespace News;

using News.ViewModels;

public class Navigator : INavigate
{
    public async Task NavigateTo(string route) => await Shell.Current.GoToAsync(route);

    public async Task PushModal(Page page) => await Shell.Current.Navigation.PushModalAsync(page);

    public async Task PopModal() => await Shell.Current.Navigation.PopModalAsync();
}
