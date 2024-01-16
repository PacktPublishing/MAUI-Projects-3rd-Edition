using CommunityToolkit.Mvvm.ComponentModel;
using SticksAndStones.Models;

namespace SticksAndStones.ViewModels;

public partial class MatchPlayerViewModel: ObservableObject
{
    private readonly Player playerModel;
    private readonly Match matchModel;

    public MatchPlayerViewModel(Player player, Match match)
    {
        this.playerModel = player;
        this.matchModel = match;
    }

    public int PlayerToken => playerModel.Id == matchModel.PlayerOneId ? 1 : -1;

    public bool IsPlayersTurn => playerModel.Id == matchModel.NextPlayerId;

    public Guid Id => playerModel.Id;

    public string GamerTag => playerModel.GamerTag;

    public string EmailAddress => playerModel.EmailAddress;

    public int Score => playerModel.Id == matchModel.PlayerOneId ? matchModel.PlayerOneScore : matchModel.PlayerTwoScore;

}
