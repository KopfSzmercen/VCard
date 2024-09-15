using MassTransit;
using VCard.Common.Application.EventBus;

namespace VCard.Common.Infrastructure.EventBus;

internal sealed class EventBus(IPublishEndpoint bus) : IEventBus
{
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        await bus.Publish(@event, cancellationToken);
    }
}