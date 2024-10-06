using EventStore.Client;
using VCard.Cards.Api.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace VCard.Cards.Api.Cards.Persistence;

internal sealed class EventStoreDbRepository(
    EventStoreClient eventStoreClient
) : IEventStoreDbRepository<Card>
{
    public async Task<Card?> FindAsync(
        Func<Card?, object, Card> when,
        string id,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var readResult = eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                GetStreamName(id),
                StreamPosition.Start,
                cancellationToken: cancellationToken
            );

            return await readResult
                .Select(x => x.Deserialize())
                .AggregateAsync(default, when!, cancellationToken);
        }
        catch (StreamNotFoundException)
        {
            return null;
        }
    }

    public async Task AppendAsync(
        string id,
        object @event,
        CancellationToken cancellationToken
    )
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            @event.GetType().Name,
            JsonSerializer.SerializeToUtf8Bytes(@event)
        );

        await eventStoreClient.AppendToStreamAsync(
            GetStreamName(id),
            StreamState.NoStream,
            [eventData],
            cancellationToken: cancellationToken
        );
    }

    public Task AppendAsync(
        string id,
        object @event,
        uint version,
        CancellationToken cancellationToken
    )
    {
        var eventData = new EventData(
            Uuid.NewUuid(),
            @event.GetType().Name,
            JsonSerializer.SerializeToUtf8Bytes(@event)
        );

        return eventStoreClient.AppendToStreamAsync(
            GetStreamName(id),
            StreamRevision.FromInt64(version),
            [eventData],
            cancellationToken: cancellationToken
        );
    }

    private static string GetStreamName(string id)
    {
        return $"card-{id}";
    }
}