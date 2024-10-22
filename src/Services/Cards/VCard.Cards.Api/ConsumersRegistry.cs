using MassTransit;
using VCard.Cards.Api.Presentation;
using VCard.Common.Infrastructure.Consumers;
using VCard.Users.IntegrationEvents;

namespace VCard.Cards.Api;

internal static class ConsumersRegistry
{
    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator, string instanceId)
    {
        registrationConfigurator
            .AddConsumer<IntegrationEventConsumer<UserRegistered>>();

        registrationConfigurator
            .AddConsumer<WithdrawMoneyOnEmailSentHandler>();
    }
}