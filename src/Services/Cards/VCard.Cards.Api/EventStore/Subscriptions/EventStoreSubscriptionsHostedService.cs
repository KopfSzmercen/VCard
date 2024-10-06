namespace VCard.Cards.Api.EventStore.Subscriptions;

internal sealed class EventStoreSubscriptionsHostedService(
    EventStoreSubscriptions eventStoreSubscriptions
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return eventStoreSubscriptions.SubscribeToAllAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return eventStoreSubscriptions.StopAsync();
    }
}