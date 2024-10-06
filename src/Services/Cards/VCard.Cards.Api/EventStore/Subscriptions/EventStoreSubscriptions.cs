using EventStore.Client;
using VCard.Cards.Api.EventStore.Checkpoints;

namespace VCard.Cards.Api.EventStore.Subscriptions;

internal sealed class EventStoreSubscriptions(
    EventStoreClient client,
    IEventStoreEventsProcessor eventsProcessor,
    IServiceScopeFactory serviceScopeFactory
)
{
    private Task? _subscriptionTask;
    private CancellationTokenSource _cts = new();
    public const string SubscriptionId = "subscriptionId";

    public Task SubscribeToAllAsync()
    {
        _subscriptionTask = ProcessSubscriptionAsync(_cts.Token);
        return Task.CompletedTask;
    }

    private async Task ProcessSubscriptionAsync(CancellationToken cancellationToken)
    {
        try
        {
            Checkpoint? lastCheckpoint;

            await using (var scope = serviceScopeFactory.CreateAsyncScope())
            {
                var checkpointRepository = scope.ServiceProvider.GetRequiredService<ICheckpointRepository>();
                lastCheckpoint = await checkpointRepository.GetLastCheckpointAsync(SubscriptionId, cancellationToken);
            }

            var subscription = client.SubscribeToAll(
                lastCheckpoint is not null
                    ? FromAll.After(
                        new Position(lastCheckpoint.Position, lastCheckpoint.Position)
                    )
                    : FromAll.Start,
                cancellationToken: cancellationToken,
                filterOptions: new SubscriptionFilterOptions(
                    EventTypeFilter.ExcludeSystemEvents()
                )
            );

            await foreach (var resolvedEvent in subscription)
            {
                var checkpoint = new Checkpoint(
                    SubscriptionId,
                    resolvedEvent.OriginalPosition.GetValueOrDefault().CommitPosition,
                    DateTime.UtcNow
                );

                await eventsProcessor.ProcessAsync(resolvedEvent, checkpoint);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal cancellation, ignore
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Subscription error: {ex}");
        }
    }

    public async Task StopAsync()
    {
        if (_subscriptionTask != null)
        {
            await _cts.CancelAsync();
            await _subscriptionTask;
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }
    }
}