using CommunityToolkit.Mvvm.Messaging.Messages;
using SticksAndStones.Models;

namespace SticksAndStones.Messages;

class MatchUpdated : ValueChangedMessage<Match>
{
    public MatchUpdated(Match match) : base(match)
    {
    }
}
