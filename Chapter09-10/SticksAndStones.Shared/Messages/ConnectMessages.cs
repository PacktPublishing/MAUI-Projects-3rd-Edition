using SticksAndStones.Models;
using System.Collections.Generic;

namespace SticksAndStones.Messages;

public record struct ConnectRequest(Player Player);
public record struct ConnectResponse(Player Player, List<Player> Players, ConnectionInfo ConnectionInfo);
