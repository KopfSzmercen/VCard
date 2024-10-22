using MassTransit;
using Microsoft.EntityFrameworkCore;
using VCard.Communication.Api.Saga;

namespace VCard.Communication.Api.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EmailSendingSagaData> EmailSendingSagaData { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("communication");

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();

        builder.Entity<EmailSendingSagaData>(entity => { entity.HasKey(x => x.CorrelationId); });
    }
}