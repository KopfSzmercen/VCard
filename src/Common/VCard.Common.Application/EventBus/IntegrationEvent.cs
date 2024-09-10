namespace VCard.Common.Application.EventBus;

public abstract class IntegrationEvent(Guid id, DateTime occurredOnUtc) : IIntegrationEvent
{
    public Guid Id { get; init; } = id;

    public DateTimeOffset OccurredOn { get; init; } = occurredOnUtc;
}