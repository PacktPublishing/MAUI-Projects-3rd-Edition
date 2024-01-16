using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SticksAndStones.Messages;
using SticksAndStones.Models;
using SticksAndStones.Services;

namespace SticksAndStones.ViewModels;

public partial class ConnectViewModel : ViewModelBase
{
    private readonly GameService gameService;
    private readonly Settings settings;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string gamerTag;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string emailAddress;

    [ObservableProperty]
    private string connectStatus;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private bool isConnecting;

    public ConnectViewModel(GameService gameService, Settings settings)
    {
        CanRefresh = false;

        this.gameService = gameService;
        this.settings = settings;

        // Load Player settings
        var player = settings.LastPlayer;
        GamerTag = player.GamerTag;
        EmailAddress = player.EmailAddress;

        ConnectStatus = "Connect";

        this.IsActive = true;
    }

    protected override void OnActivated() => Messenger.Register<ServiceError>(this, (r, m) => OnServiceError(m.Value));

    protected override void OnDeactivated() => Messenger.Unregister<ServiceError>(this);


    private void OnServiceError(AsyncError error)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.CurrentPage.DisplayAlert("There is a problem...", error.Message, "Ok");
        });
    }

    private async Task<Player> Connect(Player player)
    {
        // Get SignalR Connection
        var playerUpdate = await gameService.Connect(player);

        if (gameService.IsConnected)
        {
            IsActive = false;

            // If the player has an in progress match, take them to it.
            if (gameService.CurrentPlayer?.MatchId != Guid.Empty)
            {
                await Shell.Current.GoToAsync($"///Match", new Dictionary<string, object>() { { "MatchId", gameService.CurrentPlayer.MatchId } });
            }
            else
            {
                await Shell.Current.GoToAsync($"///Lobby");
            }
        }
        return playerUpdate;
    }

    private bool CanExecuteConnect() => !string.IsNullOrEmpty(GamerTag) && !string.IsNullOrEmpty(EmailAddress) && !IsConnecting;

    [RelayCommand(CanExecute = nameof(CanExecuteConnect))]
    public async Task Connect()
    {
        IsConnecting = true;
        ConnectStatus = "Connecting...";

        var player = settings.LastPlayer;

        player.GamerTag = GamerTag;
        player.EmailAddress = EmailAddress;

        player.Id = (await Connect(player)).Id;

        settings.LastPlayer = player;

        ConnectStatus = "Connect";
        IsConnecting = false;
    }
}
