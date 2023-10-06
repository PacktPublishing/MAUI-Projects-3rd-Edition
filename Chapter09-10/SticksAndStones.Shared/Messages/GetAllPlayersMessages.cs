using SticksAndStones.Models;
using System.Collections.Generic;

namespace SticksAndStones.Messages;

public record struct GetAllPlayersResponse(List<Player> Players);
