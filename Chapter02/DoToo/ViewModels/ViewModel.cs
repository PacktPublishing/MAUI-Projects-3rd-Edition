namespace DoToo.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

[ObservableObject]
public abstract partial class ViewModel
{
    public INavigation Navigation { get; set; }
}
