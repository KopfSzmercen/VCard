using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace VCard.Common.Infrastructure.Outbox;

public class ValidateRegistrationConsumerDefinition<TConsumer, TDbContext> :
    ConsumerDefinition<TConsumer> where TConsumer : class, IConsumer where TDbContext : DbContext
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<TConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000, 1000, 1000, 1000, 1000, 2000));

        endpointConfigurator.UseEntityFrameworkOutbox<TDbContext>(context);
    }
}