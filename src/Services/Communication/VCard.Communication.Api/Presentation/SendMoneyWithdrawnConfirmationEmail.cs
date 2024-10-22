using MassTransit;
using VCard.Cards.IntegrationEvents;
using VCard.Communication.Api.EmailSender;
using VCard.Communication.IntegrationEvents;

namespace VCard.Communication.Api.Presentation;

internal sealed class SendMoneyWithdrawnConfirmationEmail(
    IEmailSender emailSender
) : IConsumer<MoneyWithdrawnIntegrationEvent>
{
    public async Task Consume(ConsumeContext<MoneyWithdrawnIntegrationEvent> context)
    {
        await emailSender.SendEmailAsync(
            "testEmail@e.pl",
            "Money withdrawn",
            $"You have withdrawn {context.Message.Amount} USD"
        );

        await context.Publish(new WithdrawMoneyConfirmationEmailSent(
            context.Message.EmailId,
            context.Message.OwnerId
        ));
    }
}