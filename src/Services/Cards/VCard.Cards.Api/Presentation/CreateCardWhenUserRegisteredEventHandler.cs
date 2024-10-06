using VCard.Cards.Api.Cards;
using VCard.Cards.Api.Cards.Creating;
using VCard.Cards.Api.Cards.Persistence;
using VCard.Common.Application.EventBus;
using VCard.Users.IntegrationEvents;

namespace VCard.Cards.Api.Presentation;

internal sealed class CreateCardWhenUserRegisteredEventHandler(
    IEventStoreDbRepository<Card> repository,
    ILogger<CreateCardWhenUserRegisteredEventHandler> logger) : IntegrationEventHandler<UserRegistered>
{
    public override async Task HandleAsync(UserRegistered @event, CancellationToken cancellationToken = default)
    {
        var cardCreatedEvent = new CardCreated(
            @event.UserId,
            @event.UserId,
            DateTimeOffset.Now,
            new Money(0, "USD")
        );

        await repository.AppendAsync(
            cardCreatedEvent.CardId.ToString(),
            cardCreatedEvent,
            cancellationToken
        );

        logger.LogInformation("Card {CardId} created for user {UserEmail}", cardCreatedEvent.CardId, @event.UserEmail);
    }
}