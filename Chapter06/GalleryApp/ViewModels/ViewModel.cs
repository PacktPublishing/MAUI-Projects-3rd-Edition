namespace GalleryApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public abstract partial class ViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;

    abstract protected internal Task Initialize();
}
