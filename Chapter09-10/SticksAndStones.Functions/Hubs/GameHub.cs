using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.SignalR.Management;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SticksAndStones.Handlers;
using SticksAndStones.Messages;
using SticksAndStones.Models;
using SticksAndStones.Repository;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SticksAndStones.Hubs;

public class GameHub : ServerlessHub
{
    private readonly JsonSerializerOptions jsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDbContextFactory<GameDbContext> contextFactory;
    private readonly ChallengeHandler challengeHandler;


    public GameHub(IDbContextFactory<GameDbContext> dbContextFactory, ChallengeHandler handler)
    {
        contextFactory = dbContextFactory;
        challengeHandler = handler;
    }


    [FunctionName("Connect")]
    public async Task<IActionResult> Connect(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        [SignalR(HubName = "GameHub")] IAsyncCollector<SignalRMessage> signalRMessages,
        ILogger log)
    {
        log.LogInformation("A new client is requesting connection");

        var result = await JsonSerializer.DeserializeAsync<ConnectRequest>(req.Body, jsonOptions);
        var newPlayer = result.Player;

        if (newPlayer is null)
        {
            var error = new ArgumentException("No player data.", "Player");
            log.LogError(error, "Failure to deserialize arguments");
            return new BadRequestObjectResult(error);
        }

        if (string.IsNullOrEmpty(newPlayer.GamerTag))
        {
            var error = new ArgumentException("A GamerTag is required for all players.", "GamerTag");
            log.LogError(error, "Invalid value for GamerTag");
            return new BadRequestObjectResult(error);
        }

        if (string.IsNullOrEmpty(newPlayer.EmailAddress))
        {
            var error = new ArgumentException("An Email Address is required for all players.", "EmailAddress");
            log.LogError(error, "Invalid value for EmailAddress");
            return new BadRequestObjectResult(error);
        }
        
        using var context = contextFactory.CreateDbContext();

        log.LogInformation("Checking for GamerTag usage");
        var gamerTagInUse = (from p in context.Players 
                             where string.Equals(p.GamerTag, newPlayer.GamerTag, StringComparison.InvariantCultureIgnoreCase) 
                             && !string.Equals(p.EmailAddress, newPlayer.EmailAddress, StringComparison.OrdinalIgnoreCase) 
                             select p).Any();
        if (gamerTagInUse)
        {
            var error = new ArgumentException($"The GamerTag {newPlayer.GamerTag} is in use, please choose another.", "GamerTag");
            log.LogError(error, "GamerTag in use.");
            return new BadRequestObjectResult(error);
        }

        log.LogInformation("Locating Player record.");
        var thisPlayer = (from p in context.Players where string.Equals(p.EmailAddress, newPlayer.EmailAddress, StringComparison.OrdinalIgnoreCase) select p).FirstOrDefault();

        if (thisPlayer is null)
        {
            log.LogInformation("Player not found, creating.");
            thisPlayer = newPlayer;
            thisPlayer.Id = Guid.NewGuid();
            context.Add(thisPlayer);
            await context.SaveChangesAsync();
        }

        log.LogInformation("Notifying connected players of new player.");
        await Clients.All.SendAsync(Constants.Events.PlayerUpdated, new PlayerUpdatedEventArgs(thisPlayer));

        // Get the set of available players
        log.LogInformation("Getting the set of available players.");
        var players = (from player in context.Players
                       where player.Id != thisPlayer.Id
                       select player).ToList();

        var connectionInfo = await NegotiateAsync(new NegotiationOptions() { UserId = thisPlayer.Id.ToString() });
        
        log.LogInformation("Creating response.");
        var connectResponse = new ConnectResponse()
        {
            Player = thisPlayer,
            Players = players,
            ConnectionInfo = new Models.ConnectionInfo { Url = connectionInfo.Url, AccessToken = connectionInfo.AccessToken }
        };

        log.LogInformation("Sending response.");
        return new OkObjectResult(connectResponse);
    }

    [FunctionName("GetAllPlayers")]
    public IActionResult GetAllPlayers(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Players/GetAll")] HttpRequest req,
    ILogger log)
    {
        // Exclude the playerId if provided
        Guid playerId = Guid.Empty;
        if (req.Query.ContainsKey("id"))
        {
            string id = req.Query["id"];
            if (!string.IsNullOrEmpty(id))
            {
                playerId = new Guid(id);
            }
        }

        using var context = contextFactory.CreateDbContext();

        // Get the set of available players
        log.LogInformation("Getting the set of available players.");
        var players = (from player in context.Players
                       where player.Id != playerId
                       select player).ToList();
        return new OkObjectResult(new GetAllPlayersResponse(players));
    }
    
    [FunctionName("IssueChallenge")]
    public async Task<IssueChallengeResponse> IssueChallenge(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = $"Challenge/Issue")] HttpRequest req,
        ILogger log)
    {
        var result = await JsonSerializer.DeserializeAsync<IssueChallengeRequest>(req.Body, jsonOptions);

        using var context = contextFactory.CreateDbContext();

        Guid challengerId = result.Challenger.Id;
        var challenger = (from p in context.Players
                          where p.Id == challengerId
                          select p).FirstOrDefault();

        Guid opponentId = result.Opponent.Id;
        var opponent = (from p in context.Players
                        where p.Id == opponentId
                        select p).FirstOrDefault();

        if (challenger is null)
            throw new ArgumentException(paramName: nameof(challenger), message: $"{challenger.GamerTag} is not a valid player.");
        if (opponent is null)
            throw new ArgumentException(paramName: nameof(opponent), message: $"{opponent.GamerTag} is not a valid player.");

        var challengerInMatch = (from g in context.Matches
                                where g.PlayerOneId == challengerId || g.PlayerTwoId == challengerId
                                select g).Any();

        var opponentInMatch = (from g in context.Matches
                              where g.PlayerOneId == opponentId || g.PlayerTwoId == opponentId
                              select g).Any();

        if (challengerInMatch)
            throw new ArgumentException(paramName: nameof(challenger), message: $"{challenger.GamerTag} is already in a match!");

        if (opponentInMatch)
            throw new ArgumentException(paramName: nameof(opponent), message: $"{opponent.GamerTag} is already in a match!");

        Guid matchId = Guid.Empty;

        log.LogInformation($"{challenger.GamerTag} has challenged {opponent.GamerTag} to a match!");

        var challengeInfo = challengeHandler.CreateChallenge(challenger, opponent);
        log.LogInformation($"Challenge [{challengeInfo.id}] has been created.");

        log.LogInformation($"Waiting on response from {opponent.GamerTag} for challenge[{challengeInfo.id}].");
        await Clients.User(opponent.Id.ToString()).SendAsync(Constants.Events.Challenge, new ChallengeEventArgs(challengeInfo.id, challenger, opponent));

        ChallengeResponse response;
        try
        {
            var challenge = await challengeInfo.responseTask.ConfigureAwait(false);
            log.LogInformation($"Got response from {opponent.GamerTag} for challenge[{challengeInfo.id}].");
            response = challenge.Response;
        }
        catch 
        {
            log.LogInformation($"Never received a response from {opponent.GamerTag} for challenge[{challengeInfo.id}], it timed out.");
            response = ChallengeResponse.TimeOut;
        }
        return new(response);
    }

}
