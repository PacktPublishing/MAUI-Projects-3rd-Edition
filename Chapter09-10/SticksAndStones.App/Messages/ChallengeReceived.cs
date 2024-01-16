using CommunityToolkit.Mvvm.Messaging.Messages;
using SticksAndStones.Models;

namespace SticksAndStones.Messages;

public class ChallengeRecieved : ValueChangedMessage<Player>
{
    public Guid Id { get; init; }
    public ChallengeRecieved(Guid id, Player challenger) : base(challenger)
    {
        Id = id;
    }
}
