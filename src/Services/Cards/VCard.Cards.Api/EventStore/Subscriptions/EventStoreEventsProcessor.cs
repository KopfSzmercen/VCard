using EventStore.Client;
using VCard.Cards.Api.Events;
using VCard.Cards.Api.EventStore.Checkpoints;
using VCard.Cards.Api.Persistence;
using VCard.Cards.Api.Serialization;

namespace VCard.Cards.Api.EventStore.Subscriptions;

public interface IEventStoreEventsProcessor
{
    Task ProcessAsync(ResolvedEvent resolvedEvent, Checkpoint checkpoint);
}

internal sealed class EventStoreEventsProcessor(
    IServiceScopeFactory serviceScopeFactory
) : IEventStoreEventsProcessor
{
    public async Task ProcessAsync(ResolvedEvent resolvedEvent, Checkpoint checkpoint)
    {
        if (IsEventWithEmptyData(resolvedEvent)) return;

        var deserializedEvent = resolvedEvent.Deserialize();

        //create event envelope - deserialized event will have data since we checked for empty data earlier
        var eventEnvelope = new EventEnvelope(deserializedEvent!,
            new EventMetadata(
                resolvedEvent.Event.EventId.ToString(),
                resolvedEvent.OriginalEventNumber,
                resolvedEvent.OriginalPosition.GetValueOrDefault().CommitPosition
            )
        );

        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var handlers = GetEventHandlers(deserializedEvent!, scope);
        var concreteEventEnvelope = CreateConcreteEventEnvelope(deserializedEvent!, eventEnvelope);

        await using var transaction =
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.BeginTransactionAsync();

        try
        {
            await HandleEventAsync(handlers, concreteEventEnvelope);

            await SaveCheckpointAsync(checkpoint, scope, CancellationToken.None);

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task SaveCheckpointAsync(
        Checkpoint checkpoint,
        IServiceScope scope,
        CancellationToken cancellationToken
    )
    {
        var checkpointRepository = scope.ServiceProvider.GetRequiredService<ICheckpointRepository>();

        var lastCheckpoint =
            await checkpointRepository.GetLastCheckpointAsync(checkpoint.SubscriptionId, cancellationToken);

        if (lastCheckpoint is null)
        {
            await checkpointRepository.SaveCheckpointAsync(
                checkpoint,
                cancellationToken
            );

            return;
        }

        lastCheckpoint.MakeCheckpoint(checkpoint.Position, checkpoint.CheckpointedAt);

        await checkpointRepository.UpdateCheckpointAsync(
            lastCheckpoint,
            CancellationToken.None
        );
    }

    private static async Task HandleEventAsync(
        IEnumerable<object> handlers,
        object concreteEventEnvelope
    )
    {
        foreach (var handler in handlers)
        {
            // Get the HandleAsync method using reflection
            var methodInfo = handler.GetType().GetMethod(nameof(IEventHandler.HandleAsync));

            if (methodInfo is null) continue;

            var task = (Task)methodInfo.Invoke(
                handler,
                [concreteEventEnvelope, default(CancellationToken)]
            )!;

            await task;
        }
    }

    private static IEnumerable<object> GetEventHandlers(object deserializedEvent, IServiceScope scope)
    {
        // Get the concrete event type
        var eventType = deserializedEvent.GetType();

        // Construct the generic EventEnvelope<T> type for the concrete event type
        var envelopeType = typeof(EventEnvelope<>).MakeGenericType(eventType);

        // Create the specific EventEnvelope<T> instance
        var handlerType = typeof(IEventHandler<>).MakeGenericType(envelopeType);

        return scope.ServiceProvider.GetServices(handlerType)!;
    }

    private static object
        CreateConcreteEventEnvelope(object deserializedEvent, EventEnvelope eventEnvelope)
    {
        var envelopeType = typeof(EventEnvelope<>).MakeGenericType(deserializedEvent.GetType());

        var concreteEnvelope =
            Activator.CreateInstance(envelopeType, deserializedEvent, eventEnvelope.Metadata);

        if (concreteEnvelope is null)
        {
            throw new Exception($"Failed to create concrete envelope for event {deserializedEvent.GetType().Name}");
        }

        return concreteEnvelope;
    }

    private static bool IsEventWithEmptyData(ResolvedEvent resolvedEvent)
    {
        return resolvedEvent.Event.Data.Length == 0;
    }
}