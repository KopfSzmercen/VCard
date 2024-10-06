using VCard.Common.Application.EventBus;

namespace VCard.Users.IntegrationEvents;

public class UserRegistered : IIntegrationEvent
{
    public string UserEmail { get; init; }

    public Guid UserId { get; init; }
    public Guid Id { get; init; }

    public DateTimeOffset OccurredOn { get; init; }
}