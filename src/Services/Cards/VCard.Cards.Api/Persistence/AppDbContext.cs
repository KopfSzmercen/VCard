using Microsoft.EntityFrameworkCore;
using VCard.Cards.Api.Cards.GettingCard;
using VCard.Cards.Api.EventStore.Checkpoints;
using VCard.Cards.Api.Projections.Configurations;

namespace VCard.Cards.Api.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Checkpoint> Checkpoints { get; init; }

    public DbSet<CardResponse> CardResponses { get; init; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        base.OnModelCreating(builder);

        builder.HasDefaultSchema("cards");

        builder.ApplyProjectionsConfiguration();
    }
}