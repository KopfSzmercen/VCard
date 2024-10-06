using EventStore.Client;
using VCard.Cards.Api.Cards;
using VCard.Cards.Api.Cards.Persistence;
using VCard.Cards.Api.EventStore.Checkpoints;
using VCard.Cards.Api.EventStore.Subscriptions;

namespace VCard.Cards.Api.EventStore;

internal static class EventStoreExtensions
{
    public static IServiceCollection AddEventStoreSubscriptions(this IServiceCollection services)
    {
        services.AddSingleton<EventStoreSubscriptions>();

        services.AddHostedService<EventStoreSubscriptionsHostedService>();

        services.AddSingleton<IEventStoreEventsProcessor, EventStoreEventsProcessor>();

        services.AddScoped<ICheckpointRepository, PostgresCheckpointRepository>();

        return services;
    }

    public static IServiceCollection AddEventStore(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddSingleton(_ =>
        {
            var settings = configuration
                .GetRequiredSection(EventStoreOptions.SectionName)
                .Get<EventStoreOptions>();

            var settingsBuilder = EventStoreClientSettings
                .Create(settings!.ConnectionString);

            return new EventStoreClient(settingsBuilder);
        });

        service.AddSingleton<IEventStoreDbRepository<Card>, EventStoreDbRepository>();

        return service;
    }
}