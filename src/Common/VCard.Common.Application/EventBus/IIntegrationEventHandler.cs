namespace VCard.Common.Application.EventBus;

public interface IIntegrationEventHandler
{
    Task HandleAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
}

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IIntegrationEvent
{
    Task HandleAsync(TIntegrationEvent @event, CancellationToken cancellationToken = default);
}