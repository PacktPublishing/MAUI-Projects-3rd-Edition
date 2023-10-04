using SticksAndStones.Models;
using System;

namespace SticksAndStones.Messages;

public record struct IssueChallengeRequest(Player Challenger, Player Opponent);
public record struct IssueChallengeResponse(ChallengeResponse Response);

public record struct AcknowledgeChallengeRequest(Guid Id, ChallengeResponse Response);