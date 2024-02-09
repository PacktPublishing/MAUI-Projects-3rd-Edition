using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using SticksAndStones.Messages;
using SticksAndStones.Models;

namespace SticksAndStones.Services;

public sealed class GameService : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    private readonly ServiceConnection service;
    private readonly Settings settings;

    public Player CurrentPlayer { get; private set; } = new Player() { Id = Guid.Empty, MatchId = Guid.Empty };

    public ObservableCollection<Player> Players { get; } = new();

    public bool IsConnected { get; private set; }

    public GameService(Settings settings, ServiceConnection service)
    {
        this.service = service;
        this.settings = settings;
    }

    public void Dispose()
    {
        service.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<Player> Connect(Player player)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            CurrentPlayer = player;

            var (response, error) = await service.PostAsync<ConnectResponse>(new($"{settings.ServerUrl}/Connect"), new ConnectRequest(player));
            if (error is null)
            {
                service.ConnectHub(response.ConnectionInfo);

                response.Players.ForEach(Players.Add);
                CurrentPlayer = response.Player;
                IsConnected = true;

                (await service.Hub).On<PlayerUpdatedEventArgs>(Constants.Events.PlayerUpdated, PlayerStatusChangedHandler);
                (await service.Hub).On<ChallengeEventArgs>(Constants.Events.Challenge, (args) => WeakReferenceMessenger.Default.Send(new ChallengeRecieved(args.Id, args.Challenger)));
                (await service.Hub).On<MatchStartedEventArgs>(Constants.Events.MatchStarted, (args) => WeakReferenceMessenger.Default.Send(new MatchStarted(args.Match)));
                (await service.Hub).On<MatchUpdatedEventArgs>(Constants.Events.MatchUpdated, (args) => WeakReferenceMessenger.Default.Send(new MatchUpdated(args.Match)));

                (await service.Hub).Reconnected += (s) => { return RefreshPlayerList(); };
            }
            else
            {
                WeakReferenceMessenger.Default.Send<ServiceError>(new(error));
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
        return CurrentPlayer;
    }

    public async Task RefreshPlayerList()
    {
        await semaphoreSlim.WaitAsync();
        try
        {

            var getAllPlayers = service.GetAsync<GetAllPlayersResponse>(new($"{settings.ServerUrl}/Players/GetAll"), new Dictionary<string, string> { { "id", $"{CurrentPlayer.Id}" } });
            var (response, error) = await getAllPlayers;
            if (error is null)
            {
                Players.Clear();
                response.Players.ForEach(Players.Add);
            }
            else
            {
                WeakReferenceMessenger.Default.Send<ServiceError>(new(error));
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async Task IssueChallenge(Player opponent)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            var (response, error) = await service.PostAsync<IssueChallengeResponse>(new($"{settings.ServerUrl}/Challenge/Issue"), new IssueChallengeRequest(CurrentPlayer, opponent));
            if (error is not null)
            {
                WeakReferenceMessenger.Default.Send<ServiceError>(new(error));
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async Task SendChallengeResponse(Guid challengeId, Models.ChallengeResponse challengeResponse)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            var (response, error) = await service.PostAsync<string>(new($"{settings.ServerUrl}/Challenge/Ack"), new AcknowledgeChallengeRequest(challengeId, challengeResponse));
            if (error is not null)
            {
                WeakReferenceMessenger.Default.Send<ServiceError>(new(error));
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public async Task<(Match, string)> EndTurn(Guid matchId, int position)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            var (response, error) = await service.PostAsync<ProcessTurnResponse>(new($"{settings.ServerUrl}/Match/Move"), new ProcessTurnRequest(matchId, CurrentPlayer, position));
            if (error is not null)
            {
                return (null, error.Message);
            }
            else return (response.Match, null);
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    public Player GetPlayerById(Guid playerId)
    {
        if (playerId == CurrentPlayer.Id)
            return CurrentPlayer;
        return (from p in Players where p.Id == playerId select p).FirstOrDefault();
    }

    public async Task<Match> GetMatchById(Guid matchId)
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            var (response, error) = await service.GetAsync<GetMatchResponse>(new($"{settings.ServerUrl}/Match/{matchId}"), new());
            if (error != null) { }
            if (response.Match != null)
                return response.Match;
            return new Match();
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private void PlayerStatusChangedHandler(PlayerUpdatedEventArgs args)
    {
        var changedPlayer = (from player in Players
                             where player.Id == args.Player.Id
                             select player).FirstOrDefault();
        if (changedPlayer is not null)
        {
            changedPlayer.MatchId = args.Player.MatchId;
        }
        else if (args.Player.Id != CurrentPlayer.Id)
        {
            Players.Add(args.Player);
        }
    }

}
