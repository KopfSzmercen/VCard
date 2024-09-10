using VCard.Common.Application.EventBus;
using VCard.Communication.Api.EmailSender;
using VCard.Users.IntegrationEvents;

namespace VCard.Communication.Api.Presentation;

internal sealed class SendWelcomeEmailWhenUserRegisteredEventHandler(
    IEmailSender emailSender
) : IntegrationEventHandler<UserRegistered>
{
    public override async Task HandleAsync(UserRegistered @event, CancellationToken cancellationToken = default)
    {
        await emailSender.SendEmailAsync(@event.UserEmail, "Welcome to VCard", "Welcome to VCard");
    }
}