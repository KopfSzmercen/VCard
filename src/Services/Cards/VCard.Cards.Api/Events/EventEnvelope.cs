namespace VCard.Cards.Api.Events;

public record EventMetadata(
    string EventId,
    ulong StreamPosition,
    ulong LogPosition
);

public interface IEventEnvelope
{
    object Data { get; }
    EventMetadata Metadata { get; init; }
}

public record EventEnvelope<T>(
    T Data,
    EventMetadata Metadata
) : IEventEnvelope where T : notnull
{
    object IEventEnvelope.Data => Data;
}

public record EventEnvelope(
    object Data,
    EventMetadata Metadata
) : IEventEnvelope;