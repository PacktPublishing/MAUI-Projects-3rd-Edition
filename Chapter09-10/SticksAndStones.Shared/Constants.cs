namespace SticksAndStones;

public static class Constants
{
    public static class Events
    {
        public static readonly string PlayerUpdated = nameof(PlayerUpdated);
        public static readonly string Challenge = nameof(Challenge);
        public static readonly string MatchStarted = nameof(MatchStarted);
        public static readonly string MatchUpdated = nameof(MatchUpdated);
    }

    public class ArgumentNames
    {
        public static readonly string Match = nameof(Match);
        public static readonly string MatchId = nameof(MatchId);
    }
}
