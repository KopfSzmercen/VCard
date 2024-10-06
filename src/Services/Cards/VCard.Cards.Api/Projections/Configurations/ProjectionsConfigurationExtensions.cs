using Microsoft.EntityFrameworkCore;

namespace VCard.Cards.Api.Projections.Configurations;

internal static class ProjectionsConfigurationExtensions
{
    public static ModelBuilder ApplyProjectionsConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CardResponseConfiguration());
        modelBuilder.ApplyConfiguration(new StoredCheckpointConfiguration());

        return modelBuilder;
    }
}