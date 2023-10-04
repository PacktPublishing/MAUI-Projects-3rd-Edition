using SticksAndStones.Models;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SticksAndStones.Handlers;

public class ChallengeHandler : IDisposable
{
    private record struct ChallengeRecord(Guid Id, TaskCompletionSource<Challenge> ResponseTask, DateTime Created, Challenge Challenge);

    private readonly TimeSpan ackThreshold;
    private readonly Timer timer;

    private readonly ConcurrentDictionary<Guid, ChallengeRecord> handlers = new();

    public ChallengeHandler() : this(
        completeAcksOnTimeout: true,
        ackThreshold: TimeSpan.FromSeconds(30),
        ackInterval: TimeSpan.FromSeconds(1))
    {
    }

    public ChallengeHandler(bool completeAcksOnTimeout, TimeSpan ackThreshold, TimeSpan ackInterval)
    {
        if (completeAcksOnTimeout)
        {
            timer = new Timer(_ => CheckAcks(), state: null, dueTime: ackInterval, period: ackInterval);
        }

        this.ackThreshold = ackThreshold;
    }

    public (Guid id, Task<Challenge> responseTask) CreateChallenge(Player challenger, Player opponent)
    {
        var id = Guid.NewGuid();
        var tcs = new TaskCompletionSource<Challenge>(TaskCreationOptions.RunContinuationsAsynchronously);
        handlers.TryAdd(id, new(id, tcs, DateTime.UtcNow, new(id, challenger, opponent, ChallengeResponse.None)));
        return (id, tcs.Task);
    }

    public Challenge Respond(Guid id, ChallengeResponse response)
    {
        if (handlers.TryRemove(id, out var res))
        {
            var challenge = res.Challenge;
            challenge.Response = response;
            res.ResponseTask.TrySetResult(challenge);
            return challenge;
        }
        return new Challenge();
    }

    private void CheckAcks()
    {
        foreach (var pair in handlers)
        {
            var elapsed = DateTime.UtcNow - pair.Value.Created;
            if (elapsed > ackThreshold)
            {
                pair.Value.ResponseTask.TrySetException(new TimeoutException("Response time out"));
            }
        }
    }

    public void Dispose()
    {
        timer?.Dispose();

        foreach (var pair in handlers)
        {
            pair.Value.ResponseTask.TrySetCanceled();
        }
    }
}
