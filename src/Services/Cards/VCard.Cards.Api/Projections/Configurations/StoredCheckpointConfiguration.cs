using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VCard.Cards.Api.EventStore.Checkpoints;

namespace VCard.Cards.Api.Projections.Configurations;

internal sealed class StoredCheckpointConfiguration : IEntityTypeConfiguration<Checkpoint>
{
    public void Configure(EntityTypeBuilder<Checkpoint> builder)
    {
        builder.ToTable("Checkpoints");

        builder.HasKey(x => x.SubscriptionId);

        builder.Property(x => x.SubscriptionId)
            .ValueGeneratedNever();
    }
}