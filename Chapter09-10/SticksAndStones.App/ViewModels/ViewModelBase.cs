using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SticksAndStones.ViewModels;

public abstract partial class ViewModelBase : ObservableRecipient
{
    [ObservableProperty]
    private bool canRefresh;

    [ObservableProperty]
    private bool isRefreshing;

    private bool CanExecuteRefresh() => CanRefresh && !IsRefreshing;

    protected virtual Task RefreshInternal() => Task.CompletedTask;

    [RelayCommand(CanExecute = nameof(CanExecuteRefresh))]
    public async Task Refresh()
    {
        IsRefreshing = true;
        await RefreshInternal();
        IsRefreshing = false;
        return;
    }
}
