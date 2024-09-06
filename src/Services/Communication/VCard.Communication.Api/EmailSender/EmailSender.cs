namespace VCard.Communication.Api.EmailSender;

internal sealed class EmailSender(EmailSenderConfiguration configuration, Logger<EmailSender> logger)
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        logger.LogInformation(
            "Sending email to {Email} with subject {Subject} and message {Message} from {SenderName}",
            email,
            subject,
            message,
            configuration.SenderName
        );

        return Task.CompletedTask;
    }
}