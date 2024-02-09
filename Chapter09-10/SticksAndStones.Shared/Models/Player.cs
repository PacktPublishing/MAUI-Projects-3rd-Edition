using System;

namespace SticksAndStones.Models;

public class Player
{
    public Guid Id { get; set; } = Guid.Empty;

    public string GamerTag { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;

    public Guid MatchId { get; set; } = Guid.Empty;
}
