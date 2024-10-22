using MassTransit;
using VCard.Communication.IntegrationEvents;

namespace VCard.Communication.Tests.Unit;

internal sealed class EmailSentFaultConsumer : IConsumer<EmailSent>
{
    public async Task Consume(ConsumeContext<EmailSent> context)
    {
        var exception = new Exception("EmailSentFaultConsumerFault");
        await context.NotifyFaulted(
            TimeSpan.Zero,
            "EmailSentFaultConsumerFault",
            exception
        );

        throw exception;
    }
}