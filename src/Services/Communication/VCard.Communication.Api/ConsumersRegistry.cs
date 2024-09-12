using MassTransit;
using VCard.Common.Infrastructure.Consumers;
using VCard.Users.IntegrationEvents;

namespace VCard.Communication.Api;

internal static class ConsumersRegistry
{
    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator, string instanceId)
    {
        registrationConfigurator.AddConsumer<IntegrationEventConsumer<UserRegistered>>();
    }
}