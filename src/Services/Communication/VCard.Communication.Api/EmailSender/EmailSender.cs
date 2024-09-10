using Microsoft.Extensions.Options;

namespace VCard.Communication.Api.EmailSender;

internal sealed class EmailSender(IOptions<EmailSenderConfiguration> configuration, ILogger<EmailSender> logger)
    : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        logger.LogInformation(
            "Sending email to {Email} with subject {Subject} and message {Message} from {SenderName}",
            email,
            subject,
            message,
            configuration.Value.SenderName
        );

        return Task.CompletedTask;
    }
}