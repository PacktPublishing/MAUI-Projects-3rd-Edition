using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SticksAndStones.Models;
using SticksAndStones.Services;

namespace SticksAndStones.ViewModels;

public partial class PlayerViewModel: ObservableObject
{
    private readonly Player playerModel;
    private readonly GameService gameService;

    public PlayerViewModel(Player player, GameService gameService)
    {
        playerModel = player;
        this.gameService = gameService;
    }

    public Guid Id => playerModel.Id;

    public string GamerTag => playerModel.GamerTag;

    public string EmailAddress => playerModel.EmailAddress;

    public bool IsInMatch => !(playerModel.MatchId == Guid.Empty);

    public string Status => IsInMatch switch
    {
        true => "In a game",
        false => "Waiting for opponent"
    };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChallengeStatus))]
    private bool isChallenging = false;

    public string ChallengeStatus => IsChallenging switch
    {
        true => "Challenging...",
        false => "Challenge"
    };

    public bool CanChallenge => !IsInMatch && !IsChallenging;

    [RelayCommand(CanExecute = nameof(CanChallenge))]
    public void Challenge()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            IsChallenging = true;
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("Issue Challenge!", $"You are about to challenge {GamerTag} to a match!\nAre you sure?", "Yes", "No");
            if (answer)
            {
                await gameService.IssueChallenge(playerModel);
            }
            IsChallenging = false;
        });
        return;
    }
}
