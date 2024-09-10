using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VCard.Common.Application.EventBus;

namespace VCard.Common.Infrastructure.Consumers;

public sealed class IntegrationEventConsumer<TIntegrationEvent>(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<IntegrationEventConsumer<TIntegrationEvent>> logger)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();

        var handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler<TIntegrationEvent>>()
            .ToArray();

        if (handlers.Length < 1)
        {
            logger.LogError("Handler for {IntegrationEvent} not found", typeof(TIntegrationEvent).Name);
            return;
        }

        foreach (var handler in handlers) await handler.HandleAsync(context.Message, context.CancellationToken);
    }
}