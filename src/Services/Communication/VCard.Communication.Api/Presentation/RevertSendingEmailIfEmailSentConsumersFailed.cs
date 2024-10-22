using MassTransit;
using VCard.Communication.IntegrationEvents;

namespace VCard.Communication.Api.Presentation;

internal sealed class RevertSendingEmailIfEmailSentConsumersFailed : IConsumer<EmailSentConsumersFailed>
{
    public async Task Consume(ConsumeContext<EmailSentConsumersFailed> context)
    {
        Console.WriteLine("Abandoning sending email if money withdraw failed");

        await context.Publish(new RevertedSendingEmail(context.Message.EmailId));
    }
}