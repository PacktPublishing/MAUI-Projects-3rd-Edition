using CommunityToolkit.Mvvm.Messaging.Messages;
using SticksAndStones.Models;

namespace SticksAndStones.Messages;

public class MatchStarted : ValueChangedMessage<Match>
{
    public MatchStarted(Match match) : base(match)
    {
    }
}
