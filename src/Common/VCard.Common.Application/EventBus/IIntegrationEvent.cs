namespace VCard.Common.Application.EventBus;

public interface IIntegrationEvent
{
    Guid Id { get; }

    DateTimeOffset OccurredOn { get; }
}