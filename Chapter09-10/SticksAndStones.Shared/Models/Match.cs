using System;
using System.Collections.Generic;

namespace SticksAndStones.Models;

public class Match
{
    public Guid Id { get; set; } = Guid.Empty;

    public Guid PlayerOneId { get; set; }
    public int PlayerOneScore { get; set; }

    public Guid PlayerTwoId { get; set; }
    public int PlayerTwoScore { get; set; }

    public Guid NextPlayerId { get; set; }

    public List<int> Sticks { get; set; } = new(new int[24]);
    public List<int> Stones { get; set; } = new(new int[9]);

    public bool Completed { get; set; } = false;
    public Guid WinnerId { get; set; } = Guid.Empty;

    public static Match New(Guid challenger, Guid opponent)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            PlayerOneId = opponent,
            PlayerTwoId = challenger,
            NextPlayerId = opponent
        };
    }
}
