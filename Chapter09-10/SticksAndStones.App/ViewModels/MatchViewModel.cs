using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SticksAndStones.Models;
using SticksAndStones.Services;

namespace SticksAndStones.ViewModels;

public partial class MatchViewModel : ViewModelBase, IQueryAttributable
{
    private readonly GameService gameService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCurrentPlayersTurn))]
    private Match match;

    [ObservableProperty]
    private MatchPlayerViewModel[] players;

    public bool IsCurrentPlayersTurn => gameService.CurrentPlayer.Id == (Match?.NextPlayerId ?? Guid.Empty);

    public MatchViewModel(GameService gameService)
    {
        this.gameService = gameService;
        this.IsActive = true;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {

            Match match = null;
            if (query.ContainsKey(Constants.ArgumentNames.Match))
            {
                match = query[Constants.ArgumentNames.Match] as Match;
            }
            if (query.ContainsKey(Constants.ArgumentNames.MatchId))
            {
                var matchId = new Guid($"{query[Constants.ArgumentNames.MatchId]}");
                if (matchId != Guid.Empty)
                {
                    match = await gameService.GetMatchById(matchId);
                }
            }

            LoadMatch(match);
        });
    }

    private void LoadMatch(Match match)
    {
        if (match is null) return;

        Players = new[] {
            new MatchPlayerViewModel(gameService.GetPlayerById(match.PlayerOneId), match),
            new MatchPlayerViewModel(gameService.GetPlayerById(match.PlayerTwoId), match)
         };

        this.Match = match;
    }

    int lastSelectedStick = -1;

    [RelayCommand(CanExecute = nameof(IsCurrentPlayersTurn))]
    private void SelectStick(string arg)
    {
        if (gameService.CurrentPlayer is null) return;
        if (Match is null) return;

        if (int.TryParse(arg, out var pos))
        {
            pos--; // adjust for 0 based indexes
            if (lastSelectedStick != -1 && lastSelectedStick != pos)
                Match.Sticks[lastSelectedStick] = 0;

            if (Match.Sticks[pos] != 0)
                return;

            Match.Sticks[pos] = gameService.CurrentPlayer.Id == Match.PlayerOneId ? Players[0].PlayerToken : Players[1].PlayerToken;
            lastSelectedStick = pos;
            OnPropertyChanged(nameof(Match));
        }
    }

    [RelayCommand]
    private async Task Play()
    {
        if (lastSelectedStick == -1)
        {
            await Shell.Current.CurrentPage.DisplayAlert("Make a move", "You must make a move before you play.", "Ok");
            return;
        }
        if (await Shell.Current.CurrentPage.DisplayAlert("Make a move", "Are you sure this is the move you want, this can't be undone.", "Yes", "No"))
        {
            var (newMatch, error) = await gameService.EndTurn(Match.Id, lastSelectedStick);
            if (error is not null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Error in move", error, "Ok");
                return;
            }
            lastSelectedStick = -1;
        }
    }

    [RelayCommand]
    private async Task Undo()
    {
        if (lastSelectedStick != -1)
        {
            if (await Shell.Current.CurrentPage.DisplayAlert("Undo your move", "Are you sure you don't want to play this move?", "Yes", "No"))
            {
                OnPropertyChanging(nameof(Match));
                Match.Sticks[lastSelectedStick] = 0;
                OnPropertyChanged(nameof(Match));
                lastSelectedStick = -1;
                return;
            }
        }
    }

    [RelayCommand]
    private async Task Forfeit()
    {
        var returnToLobby = true;

        if (!Match.Completed)
        {
            returnToLobby = await Shell.Current.CurrentPage.DisplayAlert("W A I T", "Returning to the Lobby will forfeit your match, are you sure you want to do that?", "Yes", "No");
        }
        if (returnToLobby)
        {
            await Shell.Current.GoToAsync("///Lobby");
        }
    }

    protected override void OnActivated()
    {
        Messenger.Register(this, (MessageHandler<object, Messages.MatchUpdated>)OnMatchUpdated);
    }

    protected override void OnDeactivated()
    {
        Messenger.Unregister<Messages.MatchUpdated>(this);
    }

    void OnMatchUpdated(object r, Messages.MatchUpdated m)
    {
        LoadMatch(m.Value);
        if (Match.WinnerId != Guid.Empty && Match.Completed == true)
        {
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Match.WinnerId == gameService.CurrentPlayer.Id)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Congratulations!", $"You are victorious!\nPress the back button to return to the lobby.", "Ok");
                }
                else
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Bummer!", $"You were defeated, better luck next time!\nPress the back button to return to the lobby.", "Ok");
                }
            });
            return;
        }
    }
}
