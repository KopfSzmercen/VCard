using Microsoft.EntityFrameworkCore;

namespace VCard.Cards.Api.Persistence;

internal static class PersistenceExtensions
{
    public static IServiceCollection AddPostgresPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetRequiredSection(PostgresOptions.SectionName)
                    .Get<PostgresOptions>()!
                    .ConnectionString));

        return services;
    }
}