using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.Messaging;
using SticksAndStones.Messages;
using SticksAndStones.Models;
using SticksAndStones.Services;

namespace SticksAndStones.ViewModels;

public partial class LobbyViewModel : ViewModelBase
{
    private readonly GameService gameService;

    public ObservableCollection<PlayerViewModel> Players { get; init; }

    public LobbyViewModel(GameService gameService)
    {
        this.gameService = gameService;
        Players = new(from p in gameService.Players
                      where p.Id != gameService.CurrentPlayer.Id
                      select new PlayerViewModel(p, gameService));
        CanRefresh = true;
        IsActive = true;
    }

    private void OnPlayersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            foreach (var player in e.NewItems.Cast<Player>())
            {
                Players.Add(new PlayerViewModel(player, gameService));
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            foreach (var player in e.OldItems.Cast<Player>())
            {
                var toRemove = Players.FirstOrDefault(p => p.Id == player.Id);
                Players.Remove(toRemove);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Replace)
        {
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            Players.Clear();
        }
    }

    protected override void OnActivated()
    {
        Messenger.Register<ChallengeRecieved>(this, (r, m) => OnChallengeReceived(m.Id, m.Value));
        Messenger.Register<MatchStarted>(this, (r, m) => OnMatchStarted(m.Value));
        Messenger.Register<ServiceError>(this, (r, m) => OnServiceError(m.Value));

        gameService.Players.CollectionChanged += OnPlayersCollectionChanged;

        // If the player has an in progress match, take them to it.
        if (gameService.CurrentPlayer?.MatchId != Guid.Empty)
        {
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                IsActive = false;
                await Shell.Current.GoToAsync($"///Match", new Dictionary<string, object>() { { Constants.ArgumentNames.MatchId, gameService.CurrentPlayer.MatchId } });
            });
        }
    }

    protected override void OnDeactivated()
    {
        gameService.Players.CollectionChanged -= OnPlayersCollectionChanged;

        Messenger.Unregister<ChallengeRecieved>(this);
        Messenger.Unregister<MatchStarted>(this);
        Messenger.Unregister<ServiceError>(this);
    }

    private void OnChallengeReceived(Guid challengeId, Player opponent)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("You have been challenged!", $"{opponent.GamerTag} has challenged you to a match of Sticks & Stones, do you accept?", "Yes", "No");
            await gameService.SendChallengeResponse(challengeId, answer ? Models.ChallengeResponse.Accepted : Models.ChallengeResponse.Declined);
        });
    }

    private void OnMatchStarted(Match match)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            IsActive = false;
            await Shell.Current.GoToAsync($"///Match", new Dictionary<string, object>() { { Constants.ArgumentNames.Match, match } });
        });
    }

    private void OnServiceError(AsyncError error)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            IsActive = false;
            await Shell.Current.CurrentPage.DisplayAlert("There is a problem...", error.Message, "Ok");
        });
    }

    protected override async Task RefreshInternal()
    {
        await gameService.RefreshPlayerList();
        return;
    }
}
