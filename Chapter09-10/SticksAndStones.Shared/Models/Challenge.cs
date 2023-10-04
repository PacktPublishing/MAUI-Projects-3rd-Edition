using System;

namespace SticksAndStones.Models;

public record struct Challenge(Guid Id, Player Challenger, Player Opponent, ChallengeResponse Response);
