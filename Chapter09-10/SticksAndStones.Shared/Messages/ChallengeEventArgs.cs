using SticksAndStones.Models;
using System;

namespace SticksAndStones.Messages;

public record struct ChallengeEventArgs(Guid Id, Player Challenger, Player Opponent);
