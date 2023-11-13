namespace News.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

[ObservableObject]
public abstract partial class ViewModel
{
    public INavigate Navigation { get; init; }

    internal ViewModel(INavigate navigation) => Navigation = navigation;
}
