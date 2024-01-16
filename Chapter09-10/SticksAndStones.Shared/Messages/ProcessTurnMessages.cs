using SticksAndStones.Models;
using System;

namespace SticksAndStones.Messages;

public record struct ProcessTurnRequest(Guid MatchId, Player Player, int Position);
public record struct ProcessTurnResponse(Match Match);
