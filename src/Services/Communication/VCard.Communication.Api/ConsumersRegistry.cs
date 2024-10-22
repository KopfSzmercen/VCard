using MassTransit;
using VCard.Common.Infrastructure.Consumers;
using VCard.Common.Infrastructure.Outbox;
using VCard.Communication.Api.Persistence;
using VCard.Communication.Api.Presentation;
using VCard.Users.IntegrationEvents;

namespace VCard.Communication.Api;

internal static class ConsumersRegistry
{
    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator, string instanceId)
    {
        registrationConfigurator
            .AddConsumer<IntegrationEventConsumer<UserRegistered>, ValidateRegistrationConsumerDefinition<
                IntegrationEventConsumer<UserRegistered>, AppDbContext>>();

        registrationConfigurator.AddConsumer<SendMoneyWithdrawnConfirmationEmail>();

        registrationConfigurator.AddConsumer<RevertSendingEmailIfEmailSentConsumersFailed>();
    }
}