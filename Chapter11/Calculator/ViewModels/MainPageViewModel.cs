using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Calculator.ViewModels;

public partial class MainPageViewModel
{
    IMessenger messenger;

    public MainPageViewModel(Calculations results, IMessenger messenger)
    {
        Results = results;
        this.messenger = messenger;
    }

    public Calculations Results { get; init; }

    [RelayCommand]
    public void Recall(Calculation sender)
    {
        messenger.Send(sender);
    }
}
