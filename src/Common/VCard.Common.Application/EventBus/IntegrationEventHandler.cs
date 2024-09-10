namespace VCard.Common.Application.EventBus;

public abstract class IntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public abstract Task HandleAsync(TIntegrationEvent @event, CancellationToken cancellationToken = default);

    public Task HandleAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        return HandleAsync((TIntegrationEvent)@event, cancellationToken);
    }
}