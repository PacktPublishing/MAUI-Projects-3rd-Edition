using Microsoft.EntityFrameworkCore;
using SticksAndStones.Models;
using System;
using System.Linq;

namespace SticksAndStones.Repository;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Player> Players { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Match>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<Match>()
        .Property(p => p.Sticks)
        .HasConversion(
            toDb => string.Join(",", toDb),
            fromDb => fromDb.Split(',', StringSplitOptions.None).Select(int.Parse).ToList() ?? new(new int[24]));

        modelBuilder.Entity<Match>()
        .Property(p => p.Stones)
        .HasConversion(
            toDb => string.Join(",", toDb),
            fromDb => fromDb.Split(',', StringSplitOptions.None).Select(int.Parse).ToList() ?? new(new int[9]));

        base.OnModelCreating(modelBuilder);
    }
}